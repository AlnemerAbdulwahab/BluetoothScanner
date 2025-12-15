using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Enumeration;
using System.Linq;

namespace BluetoothScanner
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<BluetoothDeviceInfo> devices;
        private DeviceWatcher deviceWatcher;
        private BluetoothLEAdvertisementWatcher bleWatcher;

        public MainWindow()
        {
            InitializeComponent();
            devices = new ObservableCollection<BluetoothDeviceInfo>();
            DeviceListBox.ItemsSource = devices;
        }

        private async void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            await StartScanning();
        }

        private async Task StartScanning()
        {
            // Clear previous results
            devices.Clear();

            // Show loading state
            ScanButton.Visibility = Visibility.Collapsed;
            LoadingPanel.Visibility = Visibility.Visible;
            ResultsPanel.Visibility = Visibility.Collapsed;
            RescanButton.Visibility = Visibility.Collapsed;

            try
            {
                // Stop any existing watchers
                StopWatchers();

                // Method 1: Scan for paired and discoverable Bluetooth devices
                await ScanPairedAndDiscoverableDevices();

                // Method 2: Scan for BLE devices (including iPhone)
                ScanBLEDevices();

                // Scan for 8 seconds to catch more devices
                await Task.Delay(8000);

                // Stop all watchers
                StopWatchers();

                // Show results
                await Dispatcher.InvokeAsync(() =>
                {
                    LoadingPanel.Visibility = Visibility.Collapsed;

                    if (devices.Count > 0)
                    {
                        ResultsPanel.Visibility = Visibility.Visible;
                        ResultsHeader.Text = $"Found {devices.Count} Device(s):";
                    }
                    else
                    {
                        ResultsPanel.Visibility = Visibility.Visible;
                        ResultsHeader.Text = "No devices found. Make sure Bluetooth is enabled and devices are discoverable.";
                    }

                    RescanButton.Visibility = Visibility.Visible;
                });
            }
            catch (Exception ex)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    LoadingPanel.Visibility = Visibility.Collapsed;
                    MessageBox.Show($"Error scanning for devices: {ex.Message}\n\nTroubleshooting:\n" +
                        "1. Make sure Bluetooth is enabled\n" +
                        "2. Ensure devices are in pairing/discoverable mode\n" +
                        "3. Try running the app as Administrator",
                        "Scanning Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    ScanButton.Visibility = Visibility.Visible;
                });
            }
        }

        private async Task ScanPairedAndDiscoverableDevices()
        {
            try
            {
                // Scan for all Bluetooth devices (Classic and LE)
                string[] requestedProperties = {
                    "System.Devices.Aep.DeviceAddress",
                    "System.Devices.Aep.IsConnected",
                    "System.Devices.Aep.Bluetooth.Le.IsConnectable",
                    "System.Devices.Aep.SignalStrength"
                };

                deviceWatcher = DeviceInformation.CreateWatcher(
                    BluetoothDevice.GetDeviceSelectorFromPairingState(false),
                    requestedProperties,
                    DeviceInformationKind.AssociationEndpoint
                );

                deviceWatcher.Added += DeviceWatcher_Added;
                deviceWatcher.Updated += DeviceWatcher_Updated;
                deviceWatcher.EnumerationCompleted += (s, e) => { };
                deviceWatcher.Stopped += (s, e) => { };

                deviceWatcher.Start();

                // Also scan for paired devices
                var pairedDevices = await DeviceInformation.FindAllAsync(
                    BluetoothDevice.GetDeviceSelectorFromPairingState(true)
                );

                foreach (var device in pairedDevices)
                {
                    await Dispatcher.InvokeAsync(() =>
                    {
                        AddOrUpdateDevice(device.Name, device.Id, "Paired");
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ScanPairedAndDiscoverableDevices: {ex.Message}");
            }
        }

        private void ScanBLEDevices()
        {
            try
            {
                // Create BLE advertisement watcher for discovering nearby devices
                bleWatcher = new BluetoothLEAdvertisementWatcher
                {
                    ScanningMode = BluetoothLEScanningMode.Active
                };

                bleWatcher.Received += BleWatcher_Received;
                bleWatcher.Start();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ScanBLEDevices: {ex.Message}");
            }
        }

        private async void BleWatcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                string deviceName = string.IsNullOrEmpty(args.Advertisement.LocalName)
                    ? "Unknown BLE Device"
                    : args.Advertisement.LocalName;

                string deviceId = args.BluetoothAddress.ToString("X");
                string status = $"Signal: {args.RawSignalStrengthInDBm} dBm";

                AddOrUpdateDevice(deviceName, deviceId, status);
            });
        }

        private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            Dispatcher.InvokeAsync(() =>
            {
                string status = "Available";
                if (deviceInfo.Properties.ContainsKey("System.Devices.Aep.IsConnected"))
                {
                    var isConnected = deviceInfo.Properties["System.Devices.Aep.IsConnected"];
                    if (isConnected != null && (bool)isConnected)
                    {
                        status = "Connected";
                    }
                }

                string deviceName = string.IsNullOrEmpty(deviceInfo.Name) ? "Unknown Device" : deviceInfo.Name;
                AddOrUpdateDevice(deviceName, deviceInfo.Id, status);
            });
        }

        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            Dispatcher.InvokeAsync(() =>
            {
                var device = devices.FirstOrDefault(d => d.Id == deviceInfoUpdate.Id);
                if (device != null)
                {
                    if (deviceInfoUpdate.Properties.ContainsKey("System.Devices.Aep.IsConnected"))
                    {
                        var isConnected = deviceInfoUpdate.Properties["System.Devices.Aep.IsConnected"];
                        device.Status = (isConnected != null && (bool)isConnected) ? "Connected" : "Available";
                    }
                }
            });
        }

        private void AddOrUpdateDevice(string name, string id, string status)
        {
            // Check if device already exists
            var existingDevice = devices.FirstOrDefault(d => d.Id == id);

            if (existingDevice != null)
            {
                // Update existing device if name is better
                if (existingDevice.Name.Contains("Unknown") && !name.Contains("Unknown"))
                {
                    existingDevice.Name = name;
                }
                existingDevice.Status = status;
            }
            else
            {
                // Add new device
                devices.Add(new BluetoothDeviceInfo
                {
                    Name = name,
                    Id = id,
                    Status = status
                });
            }
        }

        private void StopWatchers()
        {
            if (deviceWatcher != null)
            {
                deviceWatcher.Added -= DeviceWatcher_Added;
                deviceWatcher.Updated -= DeviceWatcher_Updated;
                if (deviceWatcher.Status == DeviceWatcherStatus.Started ||
                    deviceWatcher.Status == DeviceWatcherStatus.EnumerationCompleted)
                {
                    deviceWatcher.Stop();
                }
                deviceWatcher = null;
            }

            if (bleWatcher != null)
            {
                bleWatcher.Received -= BleWatcher_Received;
                if (bleWatcher.Status == BluetoothLEAdvertisementWatcherStatus.Started)
                {
                    bleWatcher.Stop();
                }
                bleWatcher = null;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            StopWatchers();
            base.OnClosed(e);
        }
    }

    // Model class for Bluetooth device information
    public class BluetoothDeviceInfo : System.ComponentModel.INotifyPropertyChanged
    {
        private string name;
        private string id;
        private string status;

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Status
        {
            get => status;
            set
            {
                status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}
