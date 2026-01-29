# Modern .NET 8 WPF Desktop Application

A modern Windows desktop application built with **WPF (Windows Presentation Foundation)** and **.NET 8.0**, the successor to WinForms.

## 🚀 Features

- **Modern UI Design**: Clean, contemporary interface with rounded corners and custom styling
- **Interactive Controls**: Text input, buttons, and real-time output display
- **Action Logging**: Tracks and displays recent user actions with timestamps
- **Responsive Layout**: Organized with Grid and StackPanel layouts
- **Custom Styles**: Modern button and textbox styles with hover effects
- **SQLite Database Support**: Access and query external SQLite databases
- **Database Manager**: Full-featured database viewer and query tool

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
├── MainWindow.xaml          # Main UI layout and styling
├── MainWindow.xaml.cs       # Main window code-behind
├── DatabaseWindow.xaml      # Database manager UI
├── DatabaseWindow.xaml.cs   # Database manager code-behind
├── DatabaseHelper.cs        # SQLite database helper class
├── App.xaml                 # Application resources
├── App.xaml.cs              # Application startup code
├── ModernDesktopApp.csproj  # Project configuration
├── flex_book.bat            # Launcher batch file
├── reference/
│   └── git_note.md          # Git commands reference
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

## 🗄️ Database Manager

The application includes a powerful SQLite database manager that can access external databases.

### Features:
- **Database Connection**: Connects to `C:\Users\T917991\AppData\Roaming\com.flexdesk.app\flex_desk_db`
- **Table Browser**: View all tables in the database
- **Schema Viewer**: See column definitions for each table
- **SQL Query Editor**: Execute custom SQL queries
- **Data Grid**: View query results in a sortable, resizable grid
- **Performance Metrics**: Shows query execution time and row count

### Usage:
1. Click the "Open Database" button on the main window
2. Select a table from the left panel to view its schema
3. Use the pre-filled query or write your own SQL
4. Click "Execute Query" to run the query
5. Results appear in the data grid below

### Capabilities:
- ✅ Read data from any table
- ✅ Execute complex SELECT queries with WHERE, JOIN, etc.
- ✅ View database statistics (size, table count, row counts)
- ✅ Export-ready data grid (can be copied to Excel)
- ✅ Shared database access (other applications can use the same database)

**Note:** The database manager has read-only capabilities by default for safety. The database can be accessed simultaneously by multiple applications.

## 🎯 Next Steps

To extend this application, you could add:
- Menu bar with File/Edit/Help menus
- Toolbar with icons
- Database write operations (INSERT, UPDATE, DELETE)
- Export data to CSV/Excel
- Database backup and restore
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