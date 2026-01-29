# Modern .NET 8 WPF Desktop Application

A modern Windows desktop application built with **WPF (Windows Presentation Foundation)** and **.NET 8.0**, the successor to WinForms.

## 🚀 Features

- **Modern UI Design**: Clean, contemporary interface with rounded corners and custom styling
- **Interactive Controls**: Text input, buttons, and real-time output display
- **Action Logging**: Tracks and displays recent user actions with timestamps
- **Responsive Layout**: Organized with Grid and StackPanel layouts
- **Custom Styles**: Modern button and textbox styles with hover effects

## 🛠️ Technology Stack

- **.NET 8.0** - Latest LTS version of .NET
- **WPF** - Windows Presentation Foundation
- **XAML** - Declarative UI markup
- **C# 12** - Latest C# language features

## 📋 Requirements

- Windows 10/11
- .NET 8.0 SDK or Runtime

## 🏃 Running the Application

### Option 1: Using the batch file (Easiest!)
Simply double-click `flex_book.bat` or run in CLI:
```bash
flex_book.bat
```

### Option 2: Using dotnet CLI
```bash
dotnet run --project ModernDesktopApp.csproj
```

### Option 3: Using the executable
```bash
.\bin\Debug\net8.0-windows\ModernDesktopApp.exe
```

### Option 4: Build and Run
```bash
dotnet build ModernDesktopApp.csproj
dotnet run --project ModernDesktopApp.csproj
```

## 📁 Project Structure

```
flex_book/
├── MainWindow.xaml          # UI layout and styling
├── MainWindow.xaml.cs       # Code-behind with event handlers
├── App.xaml                 # Application resources
├── App.xaml.cs              # Application startup code
├── ModernDesktopApp.csproj  # Project configuration
└── README.md                # This file
```

## 🎨 UI Components

### Header Section
- Application title and description

### Main Content
- **Input Field**: Text input for user's name
- **Buttons**: 
  - "Say Hello" - Greets the user
  - "Clear" - Resets the form
- **Output Area**: Displays greeting messages
- **Action Log**: Shows timestamped list of recent actions

### Footer
- Technology badges showing .NET 8.0, WPF, and Modern UI Design

## 💡 Key Differences from WinForms

1. **XAML-based UI**: Declarative markup instead of designer-generated code
2. **Better Styling**: Rich styling system with templates and triggers
3. **Data Binding**: More powerful two-way data binding capabilities
4. **Vector Graphics**: Resolution-independent UI rendering
5. **Modern Architecture**: Separation of UI (XAML) and logic (C#)

## 🔧 Customization

The application uses custom styles defined in `MainWindow.xaml`:
- `ModernButton`: Styled buttons with hover effects
- `ModernTextBox`: Rounded textboxes with padding

You can modify colors, fonts, and layouts directly in the XAML file.

## 📝 Code Highlights

### Event Handlers
- `SayHelloButton_Click`: Validates input and displays greeting
- `ClearButton_Click`: Resets form fields
- `AddAction`: Logs user actions with timestamps

### Features Demonstrated
- Input validation
- Dynamic UI updates
- List management (keeping last 10 actions)
- Color-coded feedback (blue for success, red for errors)

## 🎯 Next Steps

To extend this application, you could add:
- Menu bar with File/Edit/Help menus
- Toolbar with icons
- Multiple pages/windows
- Database connectivity
- Settings dialog
- File I/O operations
- Charts and graphs
- Custom user controls

## 📚 Learn More

- [WPF Documentation](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
- [.NET 8.0 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [XAML Overview](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/xaml/)

---

**Built with .NET 8.0 and WPF** 🎉