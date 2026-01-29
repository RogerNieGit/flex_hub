# FlexHub - Modern .NET 8 WPF Desktop Application

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

- **AI Analysis** 🤖 (NEW!): Powered by Fuelix AI
  - **Quick Analysis** with 10 pre-built analyzers:
    - 📊 Summary - Get webpage overview
    - 🔗 Extract Links - Find all links with anchor text
    - 🖼️ Extract Images - List images with alt text
    - 📝 Extract Forms - Identify form fields and purposes
    - ⚡ Extract Scripts - List external scripts
    - 🎯 SEO Analysis - Check SEO optimization
    - 🏗️ Structure Analysis - Analyze HTML semantics
    - 📧 Extract Contact Info - Find emails, phones, addresses
    - 💰 Extract Prices - Locate monetary values
    - ♿ Accessibility Check - Review accessibility
  - **Custom Queries**: Ask any question about the HTML
  - **Chat Interface**: Conversational AI analysis
  - **Export Results**: Save to TXT, MD, or JSON
  - **Powered by**: Fuelix AI (OpenAI-compatible API)

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
Simply double-click `flex_hub.bat` or run in CLI:
```bash
flex_hub.bat
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
flex_hub/
├── FlexHubWindow.xaml       # Main FlexDesk-style window
├── FlexHubWindow.xaml.cs    # Main window code-behind
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
├── flex_hub.bat             # Launcher batch file
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

### AI Analysis Tab 🤖 (NEW!)

#### Setting Up AI
1. Click **⚙️ AI Settings** button
2. Enter your Fuelix AI API key (get from https://proxy.fuelix.ai)
3. Optional: Customize base URL or model
4. Click **Test Connection** to verify
5. Click **Save** to activate AI features

#### Quick Analysis
1. Navigate to a webpage in the Browser tab
2. Switch to **🤖 AI Analysis** tab
3. Select analysis type from dropdown:
   - **📊 Summary** - Get overview of webpage content
   - **🔗 Extract Links** - List all links with context
   - **🖼️ Extract Images** - Find images with alt text
   - **📝 Extract Forms** - Analyze form fields
   - **⚡ Extract Scripts** - List external scripts
   - **🎯 SEO Analysis** - Check SEO optimization
   - **🏗️ Structure Analysis** - Review HTML structure
   - **📧 Extract Contact Info** - Find contact details
   - **💰 Extract Prices** - Locate pricing information
   - **♿ Accessibility Check** - Review accessibility
4. Click **Analyze** button
5. AI response appears in chat interface

#### Custom Queries
1. Type your question in the input box at bottom
2. Examples:
   - "What is the main purpose of this page?"
   - "Extract all product names and prices"
   - "Find all email addresses and phone numbers"
   - "Summarize the key features mentioned"
   - "What technologies is this site using?"
3. Press **Enter** or click **Ask AI**
4. View response in chat interface

#### Export Results
1. After receiving AI analysis
2. Click **💾 Export** button
3. Choose format:
   - Text File (.txt)
   - Markdown File (.md)
   - JSON File (.json)
4. Select save location
5. Analysis saved with timestamp

#### Features
- **Chat Interface**: Conversational AI interaction
- **Multiple Queries**: Ask follow-up questions
- **Context Aware**: AI has access to full HTML
- **Fast Responses**: Powered by Fuelix AI
- **Flexible Models**: Support for GPT-4, Claude, etc.

## 🔧 Key Advantages Over Legacy Solutions

### WebView2 vs GeckoFX (Firefox)
| Feature | WebView2 (FlexHub) | GeckoFX (Legacy) |
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

### Git Workflow & Branch Strategy

This project uses a **feature branch workflow** with pull requests:

#### Branch Structure
- **`main`** - Production-ready code, stable releases
- **`FB-V1.0.0`** - Active development branch for version 1.0.0
- **Feature branches** - Individual feature development (if needed)

#### Development Workflow

1. **Make Changes on Development Branch**
   ```bash
   # Ensure you're on the development branch
   git checkout FB-V1.0.0
   
   # Make your changes to files
   # Test your changes locally
   
   # Stage and commit changes
   git add .
   git commit -m "Description of changes"
   ```

2. **Push to Remote**
   ```bash
   # Push development branch to GitHub
   git push origin FB-V1.0.0
   ```

3. **Create Pull Request**
   - Go to: https://github.com/RogerNieGit/flex_hub
   - Click "Pull requests" → "New pull request"
   - Set **base**: `main` ← **compare**: `FB-V1.0.0`
   - Add description of changes
   - Click "Create pull request"
   - Review and merge when ready

4. **After Merge** (optional)
   ```bash
   # Switch to main and pull latest
   git checkout main
   git pull origin main
   
   # Update development branch with merged changes
   git checkout FB-V1.0.0
   git merge main
   ```

#### Quick Commands Reference
```bash
# Check current branch
git branch

# Check status
git status

# View commit history
git log --oneline -10

# Push current branch
git push origin $(git branch --show-current)

# Create and push new feature branch
git checkout -b feature/my-feature
git push -u origin feature/my-feature
```

#### Best Practices
- ✅ Always work on `FB-V1.0.0` branch for development
- ✅ Keep `main` branch stable and production-ready
- ✅ Use descriptive commit messages
- ✅ Test changes before pushing
- ✅ Create PRs for code review before merging to main
- ✅ Pull latest changes before starting new work

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

**FlexHub v1.0.0** - Built with .NET 8.0, WPF, and Microsoft Edge WebView2 🎉

*Inspired by FlexDesk layout design | Modern, Fast, Powerful*