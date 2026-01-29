# FlexBook - Modern .NET 8 WPF Desktop Application

**Version 1.0.0**

A modern Windows desktop application built with **WPF (Windows Presentation Foundation)** and **.NET 8.0**, featuring a FlexDesk-inspired layout and powerful web analysis tools.

## 🚀 Features

### **FlexDesk-Style Interface**
- **Menu Bar** (35px): File, Edit, View, Help menus with full functionality
- **Docker Bar** (48px): Icon-based navigation with blue theme
- **Dynamic Title Header** (60px): Context-aware header with icon, title, and subtitle
- **Main Content Area**: Flexible workspace with frame-based navigation
- **Sidebar** (300px): Context-sensitive information panel

### **Web Analyzer** 🌐
- **Modern Web Browser**: Microsoft Edge WebView2 (Chromium-based)
  - No iframe/connection restrictions
  - Full navigation controls (back, forward, refresh)
  - URL bar with auto-https
  - Loading indicator with progress
  
- **View Source**: Professional HTML source viewer
  - Syntax highlighting (Consolas font)
  - **Search Functionality**:
    - Real-time search as you type
    - Case-insensitive matching
    - Navigation buttons (▲ Previous, ▼ Next)
    - Match counter display (e.g., "3 of 15")
    - Keyboard shortcuts (Enter, Shift+Enter, Escape)
    - Auto-scroll to highlighted matches
    - Text selection highlighting

### **File & Folder Management**
- Open and analyze files with detailed metadata
- Browse folders with file/subfolder statistics
- Dynamic sidebar updates with contextual information

### **Additional Features**
- Custom window chrome with minimize/maximize/close controls
- Draggable menu bar
- Modern dark theme (#1E1E1E, #252526, #4A9EFF)
- Responsive layout design

## 🛠️ Technology Stack

- **.NET 8.0** - Latest LTS version of .NET
- **WPF** - Windows Presentation Foundation
- **XAML** - Declarative UI markup
- **C# 12** - Latest C# language features
- **Microsoft.Web.WebView2** - Modern Chromium-based browser control
- **System.Data.SQLite** - SQLite database support

## 📋 Requirements

- Windows 10/11
- .NET 8.0 SDK or Runtime
- Microsoft Edge WebView2 Runtime (usually pre-installed on Windows 10/11)

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
├── FlexBookWindow.xaml       # Main FlexDesk-style window
├── FlexBookWindow.xaml.cs    # Main window code-behind
├── WebAnalyzerPage.xaml      # Web analyzer page UI
├── WebAnalyzerPage.xaml.cs   # Web analyzer code-behind
├── MainWindow.xaml           # Original demo window
├── MainWindow.xaml.cs        # Demo window code-behind
├── DatabaseWindow.xaml       # Database manager UI
├── DatabaseWindow.xaml.cs    # Database manager code-behind
├── DatabaseHelper.cs         # SQLite database helper class
├── App.xaml                  # Application resources
├── App.xaml.cs               # Application startup code
├── ModernDesktopApp.csproj   # Project configuration
├── flex_book.bat             # Launcher batch file
├── reference/
│   ├── git_note.md           # Git commands reference
│   └── firefox_browser_integration_analysis.md
└── README.md                 # This file
```

## 🎨 FlexDesk Layout Components

### Docker Bar Icons
- 🏠 **Home** - Welcome page
- 🌐 **Web Analyzer** - Browse and analyze web pages
- 📝 **Quick Book** - Note-taking (coming soon)
- 📊 **Analytics** - Data visualization (coming soon)
- 👤 **Profile** - User settings (coming soon)
- ⚙️ **Settings** - Application configuration (coming soon)

### Menu System
- **File**: Open File, Open Folder, Exit
- **Edit**: Settings
- **View**: Zoom In, Zoom Out, Reset Zoom
- **Help**: About

## 🌐 Web Analyzer Guide

### Browser Tab
1. Enter URL in the text box (e.g., `www.google.com`)
2. Press **Enter** or click **🔍** to navigate
3. Use **◀ Back** and **▶ Forward** to navigate history
4. Click **🔄 Refresh** to reload the page

### View Source Tab
1. Switch to **📄 View Source** tab to see HTML
2. Use the search box to find specific text:
   - Type search term (e.g., `<script>`, `class=`, `div`)
   - Results appear instantly with counter
   - Click **▼** or press **Enter** for next match
   - Click **▲** or press **Shift+Enter** for previous match
   - Press **Escape** to clear search
3. Source code is syntax-highlighted for readability

### Common Search Examples
- `<div>` - Find all div tags
- `class=` - Find all class attributes
- `script` - Find all script references
- `href=` - Find all links
- `function` - Find JavaScript functions

## 🔧 Key Advantages Over Legacy Solutions

### WebView2 vs GeckoFX (Firefox)
| Feature | WebView2 (FlexBook) | GeckoFX (Legacy) |
|---------|---------------------|------------------|
| Browser Engine | Chromium (Edge) | Gecko (Firefox) |
| .NET Support | .NET 8 WPF | .NET Framework WinForms |
| Runtime | Built into Windows | Requires 50+ Firefox DLLs |
| iframe Support | ✅ Full support | ❌ Connection rejections |
| Updates | Auto-updates with Edge | Manual DLL updates |
| Modern Websites | ✅ Full compatibility | ⚠️ Older engine |
| File Size | Small (uses system) | Large (150+ MB) |

## 💡 Color Palette

The application uses a consistent dark theme:
- **Backgrounds**: #1E1E1E, #2D2D30, #252526, #2B2B2B, #3C3C3C
- **Accent Blue**: #4A9EFF (primary), #007ACC (hover)
- **Text Colors**: #CCCCCC (primary), #888888 (secondary), White (headers)
- **Borders**: #3C3C3C, #1E1E1E
- **Syntax Highlighting**: #CE9178 (HTML/code)

## 🎯 Version 1.0.0 Features

### ✅ Implemented
- FlexDesk-style layout with docker bar and sidebar
- Web Analyzer with WebView2 browser
- HTML source viewer with search functionality
- File and folder browsing with metadata
- Custom window controls
- Dynamic header updates
- Context-aware sidebar
- Menu system with keyboard shortcuts

### 🔜 Planned for Future Versions
- Quick Book note-taking feature
- Analytics dashboard
- Profile management
- Advanced settings dialog
- Database write operations
- Export functionality (CSV, PDF)
- Plugin system
- Custom themes

## 📝 Development Notes

### Building from Source
```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run

# Publish a release
dotnet publish -c Release
```

### NuGet Packages
- `Microsoft.Web.WebView2` (v1.0.3719.77) - Web browser component
- `System.Data.SQLite.Core` (v1.0.119.0) - SQLite database support

## 🐛 Troubleshooting

### WebView2 Runtime Error
If you see "Failed to initialize WebView2", install the Microsoft Edge WebView2 Runtime:
- Download from: https://developer.microsoft.com/microsoft-edge/webview2/
- Or it's usually pre-installed on Windows 10/11

### Database Connection Issues
The database path is configured for: `C:\Users\T917991\AppData\Roaming\com.flexdesk.app\flex_desk_db`
Update `DatabaseWindow.xaml.cs` to point to your database location.

## 📚 Learn More

- [WPF Documentation](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
- [.NET 8.0 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [XAML Overview](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/xaml/)
- [WebView2 Documentation](https://learn.microsoft.com/en-us/microsoft-edge/webview2/)

## 🤝 Contributing

This is a personal project, but suggestions and feedback are welcome!

## 📄 License

This project is for personal use and learning purposes.

---

**FlexBook v1.0.0** - Built with .NET 8.0, WPF, and Microsoft Edge WebView2 🎉

*Inspired by FlexDesk layout design | Modern, Fast, Powerful*