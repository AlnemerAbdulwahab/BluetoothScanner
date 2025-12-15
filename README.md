# Bluetooth Scanner - .NET C# Desktop Application

A modern WPF desktop application that scans for nearby Bluetooth devices. This project demonstrates Windows Bluetooth API integration, asynchronous programming, and modern UI design in a .NET environment.

## Project Overview

This application provides a user-friendly interface to discover Bluetooth devices in your vicinity. Built as a hands-on exercise to apply Object-Oriented Programming principles and external library integration, this project combines previously learned concepts including UI design, asynchronous programming, and event handling with the Windows Bluetooth API library to create a fully functional device scanner.

## Features

- **Dual Scanning Technology**: Utilizes both Classic Bluetooth and BLE protocols for comprehensive device detection
- **Modern UI Design**: Clean, intuitive interface with loading states and smooth transitions
- **Real-time Detection**: Discovers devices as they become available during the scan
- **Device Information Display**: Shows device name, ID, and connection status
- **Paired Device Detection**: Identifies already paired devices on your system
- **Signal Strength Indicator**: Displays signal strength for BLE devices
- **Rescan Capability**: Easy one-click rescanning for updated results

## Application Flow

```
Launch App
    ↓
[Start Scanning] Button
    ↓
Loading Screen: "Scanning..."
    ↓
Device Discovery (8 seconds)
    ↓
Results Display:
├── Device Name
├── Device ID
└── Status (Connected/Available/Signal Strength)
    ↓
[Scan Again] Button
```

## Technologies Used

- **C# (.NET 6.0)**: Core programming language
- **WPF (Windows Presentation Foundation)**: UI framework
- **Windows.Devices.Bluetooth API**: Bluetooth device discovery and management
- **XAML**: UI markup language
- **Async/Await**: Asynchronous programming patterns
- **MVVM Pattern**: Observable collections for data binding

## Installation & Setup

### Prerequisites

- Windows 10 (version 1809 or later) or Windows 11
- .NET 6.0 SDK or higher ([Download here](https://dotnet.microsoft.com/download))
- Bluetooth adapter (built-in or USB)
- Visual Studio Code or Visual Studio (optional)

### Steps

1. **Clone the repository**
```bash
git clone <repository-url>
cd BluetoothScanner
```

2. **Navigate to the project folder**
```bash
cd BluetoothScanner
```

3. **Build the project**
```bash
dotnet build
```

4. **Run the application**
```bash
dotnet run
```

### Alternative: Using Visual Studio Code

```bash
# Open in VS Code
code .

# Press F5 to run with debugging
# Or use integrated terminal: dotnet run
```

## Project Structure

```
BluetoothScanner/               # Repository root
├── README.md                   # Project documentation
└── BluetoothScanner/           # .NET WPF project
    ├── MainWindow.xaml         # UI layout and design
    ├── MainWindow.xaml.cs      # Code-behind and logic
    ├── App.xaml                # Application resources
    ├── App.xaml.cs             # Application startup
    ├── BluetoothScanner.csproj # Project configuration
    ├── bin/                    # Compiled binaries (auto-generated)
    └── obj/                    # Build files (auto-generated)
```

## Code Structure

### Key Components

#### MainWindow.xaml
- Modern UI layout with three states:
  - Initial state with scan button
  - Loading state with progress indicator
  - Results state with device list

#### MainWindow.xaml.cs
- **Core Classes**:
  - `MainWindow`: Main application window and orchestration
  - `BluetoothDeviceInfo`: Data model for discovered devices
- **Key Methods**:
  - `StartScanning()`: Initiates device discovery
  - `ScanPairedAndDiscoverableDevices()`: Classic Bluetooth scanning
  - `ScanBLEDevices()`: Bluetooth Low Energy scanning
  - `AddOrUpdateDevice()`: Manages device list updates

### Scanning Methods

| Method | Description | Protocol | Use Case |
|--------|-------------|----------|----------|
| `DeviceWatcher` | Scans for Classic Bluetooth devices | Classic BT | Headphones, speakers, computers |
| `BluetoothLEAdvertisementWatcher` | Scans for BLE devices | BLE | Smartphones, fitness trackers, IoT |
| `FindAllAsync` | Retrieves paired devices | Both | Previously connected devices |

## Usage Example

1. **Launch the application**
   - Double-click the executable or run via command line

2. **Start scanning**
   - Click the "Start Scanning" button
   - Wait for the 8-second scan to complete

3. **View results**
   - Browse the list of discovered devices
   - Check device names, IDs, and connection status

4. **Rescan**
   - Click "Scan Again" to perform a fresh scan

## Bluetooth Device Detection

### Classic Bluetooth Devices
- Desktop computers
- Laptops
- Speakers and audio devices
- Game controllers
- Some older smartphones

### BLE Devices
- Modern smartphones (iPhone, Android)
- Fitness trackers (Fitbit, Apple Watch)
- Smart home devices
- Wireless earbuds
- IoT sensors

## Troubleshooting

### No devices found?
- **Check Bluetooth is enabled**: Windows Settings → Bluetooth & devices
- **Ensure devices are discoverable**: Keep device Bluetooth settings open
- **Run as Administrator**: Right-click terminal → "Run as administrator"
- **Try pairing first**: Some devices only appear after initial pairing

### Scanning error on second scan?
- **Restart the application**: Close and reopen the app
- **Check Bluetooth adapter**: Ensure it's functioning properly
- **Update drivers**: Check for Bluetooth driver updates

### iPhone not detected?
- **Keep Bluetooth settings open**: iOS devices are more discoverable when in settings
- **Enable Bluetooth**: Settings → Bluetooth → ON
- **Try pairing**: Pair with Windows first for better detection

## Technical Concepts Demonstrated

### 1. Asynchronous Programming
- Uses `async/await` for non-blocking UI operations
- Background scanning while maintaining responsive interface
- Proper task management and cancellation

### 2. Event-Driven Architecture
- Event handlers for device discovery
- UI state management through events
- Observer pattern for device updates

### 3. Windows Runtime APIs
- Integration with Windows.Devices.Bluetooth namespace
- Cross-platform API usage in .NET
- Hardware abstraction layer interaction

### 4. Data Binding
- MVVM pattern with ObservableCollection
- Automatic UI updates when data changes
- Two-way binding between view and model

### 5. Error Handling
- Try-catch blocks for robust error management
- User-friendly error messages
- Graceful degradation on failures

## Key Design Decisions

### Why Two Scanning Methods?
- **Classic Bluetooth**: Better for traditional devices and paired connections
- **BLE**: Essential for modern smartphones and low-energy devices
- **Combined approach**: Ensures maximum device detection

### Why 8-Second Scan Duration?
- Balances discovery time with user experience
- Long enough to detect most nearby devices
- Short enough to feel responsive

### Why ObservableCollection?
- Automatic UI updates when devices are added/removed
- Built-in change notification
- Perfect for real-time device discovery

## Future Enhancements

- Device filtering by type (audio, input, phone)
- Connection capability to discovered devices
- Export device list to CSV/JSON
- Historical scan data tracking
- Custom scan duration setting
- Device icon based on type
- Bluetooth version detection
- RSSI (signal strength) graphing

## Learning Outcomes

This project demonstrates the practical application of:
- **Object-Oriented Programming**: Classes, properties, inheritance, and encapsulation with `Node`, `BluetoothDeviceInfo` classes
- **External Libraries/Packages**: Integration of Windows.Devices.Bluetooth API in a .NET project
- **UI Design**: Building responsive WPF interfaces with XAML
- **Asynchronous Programming**: Using async/await patterns for non-blocking operations
- **Event Handling**: Managing device discovery events and user interactions
- **Data Binding**: Implementing observable collections for real-time UI updates
- **Error Handling**: Proper exception management and user feedback

### OOP Concepts Applied
- **Classes and Objects**: `MainWindow`, `BluetoothDeviceInfo` model classes
- **Encapsulation**: Private fields with public properties
- **Properties**: Auto-implemented and full properties with backing fields
- **Events**: INotifyPropertyChanged implementation
- **Collections**: ObservableCollection for dynamic data management

### Library Integration Skills
- Installing and referencing external packages
- Reading API documentation
- Using Windows Runtime APIs in .NET
- Managing device watchers and advertisement listeners
- Handling platform-specific functionality

## About

This project was created as part of the Tuwaiq Academy Software Development Bootcamp as a practical application task following lessons on Object-Oriented Programming and external libraries/packages. The objective was to integrate previously learned concepts (UI design, async programming, event handling) with new library integration skills to build a real-world desktop application that interacts with system hardware.

The code was AI-generated as a learning tool. The primary objective was to study and understand the code structure, analyze how external libraries are integrated into C# projects, comprehend the implementation of OOP principles and async/await patterns, and gain the ability to modify and enhance the Bluetooth scanning functionality independently through hands-on practice.
