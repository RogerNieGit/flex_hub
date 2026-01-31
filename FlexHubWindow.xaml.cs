using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModernDesktopApp;

/// <summary>
/// FlexHub Main Window - FlexDesk-style layout
/// </summary>
public partial class FlexHubWindow : Window
{
    public FlexHubWindow()
    {
        InitializeComponent();
        LoadHomePage();
    }

    #region Window Controls

    private void MenuBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            Maximize_Click(sender, e);
        }
        else
        {
            try
            {
                DragMove();
            }
            catch (InvalidOperationException)
            {
                // Ignore - happens when window is maximized
            }
        }
    }

    private void Minimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void Maximize_Click(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
            MaximizeButton.Content = "🗖";
        }
        else
        {
            WindowState = WindowState.Maximized;
            MaximizeButton.Content = "🗗";
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        System.Windows.Application.Current.Shutdown();
    }

    #endregion

    #region Menu Bar Handlers

    private void OpenFile_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "Open File",
            Filter = "HTML Files (*.html;*.htm)|*.html;*.htm|All Files (*.*)|*.*|Text Files (*.txt)|*.txt|C# Files (*.cs)|*.cs|JSON Files (*.json)|*.json|XML Files (*.xml)|*.xml"
        };

        if (dialog.ShowDialog() == true)
        {
            string fileName = System.IO.Path.GetFileName(dialog.FileName);
            string fileExt = System.IO.Path.GetExtension(dialog.FileName);
            
            // Check if we're in Web Analyzer mode
            if (ContentFrame.Content is WebAnalyzerPage)
            {
                // For Web Analyzer, show tree view of parent folder
                string folderPath = System.IO.Path.GetDirectoryName(dialog.FileName) ?? "";
                BuildFileTreeView(folderPath, new[] { ".html", ".htm" }, dialog.FileName);
            }
            else
            {
                // For other modes, show file details
                string fileSize = new System.IO.FileInfo(dialog.FileName).Length.ToString("N0");
                UpdateHeader("📄", fileName, dialog.FileName);
                UpdateSidebarForFile(dialog.FileName, fileSize, fileExt);
            }
        }
    }

    private void OpenFolder_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new System.Windows.Forms.FolderBrowserDialog
        {
            Description = "Select a folder",
            ShowNewFolderButton = true
        };

        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            // Check if we're in Web Analyzer mode
            if (ContentFrame.Content is WebAnalyzerPage)
            {
                // For Web Analyzer, show tree view of HTML files with header
                BuildFileTreeViewWithHeader(dialog.SelectedPath, new[] { ".html", ".htm" }, null);
            }
            else
            {
                // For other modes, show folder details
                string folderName = System.IO.Path.GetFileName(dialog.SelectedPath);
                var dirInfo = new System.IO.DirectoryInfo(dialog.SelectedPath);
                int fileCount = dirInfo.GetFiles("*", System.IO.SearchOption.AllDirectories).Length;
                int folderCount = dirInfo.GetDirectories("*", System.IO.SearchOption.AllDirectories).Length;
                
                UpdateHeader("📁", folderName, dialog.SelectedPath);
                UpdateSidebarForFolder(dialog.SelectedPath, fileCount, folderCount);
            }
        }
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        System.Windows.Application.Current.Shutdown();
    }

    private void ZoomIn_Click(object sender, RoutedEventArgs e)
    {
        // Implement zoom in functionality
        System.Windows.MessageBox.Show("Zoom In functionality coming soon!", "Info", 
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void ZoomOut_Click(object sender, RoutedEventArgs e)
    {
        // Implement zoom out functionality
        System.Windows.MessageBox.Show("Zoom Out functionality coming soon!", "Info", 
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void ResetZoom_Click(object sender, RoutedEventArgs e)
    {
        // Implement reset zoom functionality
        System.Windows.MessageBox.Show("Reset Zoom functionality coming soon!", "Info", 
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void About_Click(object sender, RoutedEventArgs e)
    {
        System.Windows.MessageBox.Show(
            "FlexHub - Modern .NET 8 WPF Application\n\n" +
            "Built with:\n" +
            "- .NET 8.0\n" +
            "- WPF (Windows Presentation Foundation)\n" +
            "- C# 12\n\n" +
            "Inspired by FlexDesk layout design",
            "About FlexHub",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    #endregion

    #region Docker Bar Handlers

    private void Home_Click(object sender, RoutedEventArgs e)
    {
        LoadHomePage();
        HeaderSettingsButton.Visibility = Visibility.Collapsed;
    }

    private void WebAnalyzer_Click(object sender, RoutedEventArgs e)
    {
        UpdateHeader("🌐", "Web Analyzer", "Browse and analyze web pages");
        
        // Show settings button for Web Analyzer
        HeaderSettingsButton.Visibility = Visibility.Visible;
        
        var webAnalyzerPage = new WebAnalyzerPage();
        
        // Subscribe to default folder loaded event
        webAnalyzerPage.OnDefaultFolderLoaded += (folderPath) =>
        {
            BuildFileTreeViewWithHeader(folderPath, new[] { ".html", ".htm" }, null);
        };
        
        ContentFrame.Navigate(webAnalyzerPage);
        
        // Check for default folder and load it
        var defaultFolder = WebAnalyzerPage.GetDefaultFolder();
        if (!string.IsNullOrEmpty(defaultFolder) && System.IO.Directory.Exists(defaultFolder))
        {
            BuildFileTreeViewWithHeader(defaultFolder, new[] { ".html", ".htm" }, null);
        }
        else
        {
            UpdateWebAnalyzerSidebar();
        }
    }
    
    private void HeaderSettings_Click(object sender, RoutedEventArgs e)
    {
        // Get the current WebAnalyzerPage and show its settings dialog
        if (ContentFrame.Content is WebAnalyzerPage webAnalyzerPage)
        {
            ShowWebAnalyzerSettingsDialog();
        }
    }
    
    private void ShowWebAnalyzerSettingsDialog()
    {
        var dialog = new Window
        {
            Title = "Web Analyzer Settings",
            Width = 500,
            Height = 280,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = this,
            Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2D2D30")),
            ResizeMode = ResizeMode.NoResize,
            WindowStyle = WindowStyle.ToolWindow
        };
        
        var mainGrid = new System.Windows.Controls.Grid { Margin = new Thickness(20) };
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        
        // Title
        var titleText = new System.Windows.Controls.TextBlock
        {
            Text = "Configure default folder for Web Analyzer",
            FontSize = 14,
            Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#CCCCCC")),
            Margin = new Thickness(0, 0, 0, 20)
        };
        System.Windows.Controls.Grid.SetRow(titleText, 0);
        mainGrid.Children.Add(titleText);
        
        // Label
        var labelText = new System.Windows.Controls.TextBlock
        {
            Text = "Default Folder",
            FontSize = 12,
            FontWeight = FontWeights.SemiBold,
            Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#4A9EFF")),
            Margin = new Thickness(0, 0, 0, 8)
        };
        System.Windows.Controls.Grid.SetRow(labelText, 1);
        mainGrid.Children.Add(labelText);
        
        // Folder input row
        var inputGrid = new System.Windows.Controls.Grid();
        inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        System.Windows.Controls.Grid.SetRow(inputGrid, 2);
        
        var folderTextBox = new System.Windows.Controls.TextBox
        {
            Text = WebAnalyzerPage.GetDefaultFolder() ?? "",
            Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E1E1E")),
            Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#CCCCCC")),
            BorderBrush = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#3C3C3C")),
            Padding = new Thickness(10, 8, 10, 8),
            FontSize = 12,
            VerticalContentAlignment = VerticalAlignment.Center
        };
        System.Windows.Controls.Grid.SetColumn(folderTextBox, 0);
        inputGrid.Children.Add(folderTextBox);
        
        var browseBtn = new System.Windows.Controls.Button
        {
            Content = "📁 Browse",
            Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#3A3A3A")),
            Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#CCCCCC")),
            BorderThickness = new Thickness(0),
            Padding = new Thickness(12, 8, 12, 8),
            Margin = new Thickness(8, 0, 0, 0),
            Cursor = System.Windows.Input.Cursors.Hand
        };
        browseBtn.Click += (s, ev) =>
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Select default folder for HTML files",
                ShowNewFolderButton = false
            };
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folderTextBox.Text = folderDialog.SelectedPath;
            }
        };
        System.Windows.Controls.Grid.SetColumn(browseBtn, 1);
        inputGrid.Children.Add(browseBtn);
        
        var clearBtn = new System.Windows.Controls.Button
        {
            Content = "🗑️ Clear",
            Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#3A3A3A")),
            Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#CCCCCC")),
            BorderThickness = new Thickness(0),
            Padding = new Thickness(12, 8, 12, 8),
            Margin = new Thickness(8, 0, 0, 0),
            Cursor = System.Windows.Input.Cursors.Hand
        };
        clearBtn.Click += (s, ev) =>
        {
            folderTextBox.Text = "";
        };
        System.Windows.Controls.Grid.SetColumn(clearBtn, 2);
        inputGrid.Children.Add(clearBtn);
        
        mainGrid.Children.Add(inputGrid);
        
        // Description
        var descText = new System.Windows.Controls.TextBlock
        {
            Text = "Select a default folder to automatically load when clicking Web Analyzer in docker bar",
            FontSize = 11,
            Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#888888")),
            Margin = new Thickness(0, 12, 0, 0),
            TextWrapping = TextWrapping.Wrap
        };
        System.Windows.Controls.Grid.SetRow(descText, 3);
        mainGrid.Children.Add(descText);
        
        // Buttons row
        var buttonPanel = new System.Windows.Controls.StackPanel
        {
            Orientation = System.Windows.Controls.Orientation.Horizontal,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
            Margin = new Thickness(0, 20, 0, 0)
        };
        System.Windows.Controls.Grid.SetRow(buttonPanel, 4);
        
        var cancelBtn = new System.Windows.Controls.Button
        {
            Content = "Cancel",
            Width = 80,
            Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#3A3A3A")),
            Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#CCCCCC")),
            BorderThickness = new Thickness(0),
            Padding = new Thickness(12, 8, 12, 8),
            Margin = new Thickness(0, 0, 10, 0),
            Cursor = System.Windows.Input.Cursors.Hand
        };
        cancelBtn.Click += (s, ev) => dialog.Close();
        buttonPanel.Children.Add(cancelBtn);
        
        var saveBtn = new System.Windows.Controls.Button
        {
            Content = "💾 Save Settings",
            Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#4A9EFF")),
            Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White),
            BorderThickness = new Thickness(0),
            Padding = new Thickness(12, 8, 12, 8),
            Cursor = System.Windows.Input.Cursors.Hand
        };
        saveBtn.Click += (s, ev) =>
        {
            var folderPath = folderTextBox.Text.Trim();
            if (string.IsNullOrEmpty(folderPath))
            {
                WebAnalyzerPage.ClearDefaultFolder();
                System.Windows.MessageBox.Show("Default folder cleared.", "Settings Saved", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (System.IO.Directory.Exists(folderPath))
            {
                WebAnalyzerPage.SaveDefaultFolder(folderPath);
                System.Windows.MessageBox.Show("✅ Settings saved!", "Settings Saved", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                // Load tree with new folder
                BuildFileTreeViewWithHeader(folderPath, new[] { ".html", ".htm" }, null);
            }
            else
            {
                System.Windows.MessageBox.Show("The specified folder does not exist.", "Invalid Folder", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            dialog.Close();
        };
        buttonPanel.Children.Add(saveBtn);
        
        mainGrid.Children.Add(buttonPanel);
        
        dialog.Content = mainGrid;
        dialog.ShowDialog();
    }

    private void UpdateWebAnalyzerSidebar()
    {
        // Update sidebar for Web Analyzer
        SidebarContent.Children.Clear();
        
        var header = new System.Windows.Controls.TextBlock
        {
            Text = "FILES",
            FontSize = 14,
            FontWeight = FontWeights.SemiBold,
            Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#888888")),
            Margin = new Thickness(0, 0, 0, 15)
        };
        SidebarContent.Children.Add(header);

        var infoBorder = new System.Windows.Controls.Border
        {
            Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E1E1E")),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(10),
            Margin = new Thickness(0, 0, 0, 10)
        };

        var infoText = new System.Windows.Controls.TextBlock
        {
            Text = "Use File → Open File or Open Folder to load HTML files and view them in a tree structure here.",
            FontSize = 12,
            Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#888888")),
            TextWrapping = TextWrapping.Wrap
        };

        infoBorder.Child = infoText;
        SidebarContent.Children.Add(infoBorder);
    }

    private void QuickBook_Click(object sender, RoutedEventArgs e)
    {
        HeaderSettingsButton.Visibility = Visibility.Collapsed;
        UpdateHeader("📝", "Quick Book", "Note-taking and documentation");
        // TODO: Load quick book page
    }

    private void Analytics_Click(object sender, RoutedEventArgs e)
    {
        HeaderSettingsButton.Visibility = Visibility.Collapsed;
        UpdateHeader("📊", "Analytics", "Data visualization and insights");
        // TODO: Load analytics page
    }

    private void Profile_Click(object sender, RoutedEventArgs e)
    {
        HeaderSettingsButton.Visibility = Visibility.Collapsed;
        UpdateHeader("👤", "Profile", "User settings and preferences");
        // TODO: Load profile page
    }

    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        HeaderSettingsButton.Visibility = Visibility.Collapsed;
        UpdateHeader("⚙️", "Settings", "Application configuration");
        // TODO: Load settings page
    }

    #endregion

    #region Helper Methods

    private void LoadHomePage()
    {
        UpdateHeader("🏠", "Welcome to FlexHub", "Modern .NET 8 WPF Application");
        ResetSidebar();
    }

    private void UpdateHeader(string icon, string title, string subtitle)
    {
        HeaderIcon.Text = icon;
        HeaderTitle.Text = title;
        HeaderSubtitle.Text = subtitle;
    }

    private void UpdateSidebarForFile(string filePath, string fileSize, string fileExt)
    {
        SidebarContent.Children.Clear();
        
        // Header
        var header = new System.Windows.Controls.TextBlock
        {
            Text = "FILE DETAILS",
            FontSize = 14,
            FontWeight = FontWeights.SemiBold,
            Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#888888")),
            Margin = new Thickness(0, 0, 0, 15)
        };
        SidebarContent.Children.Add(header);

        // File info
        AddSidebarInfo("📄 File Name", System.IO.Path.GetFileName(filePath));
        AddSidebarInfo("📏 Size", $"{fileSize} bytes");
        AddSidebarInfo("📑 Type", fileExt.ToUpper());
        AddSidebarInfo("📍 Location", System.IO.Path.GetDirectoryName(filePath) ?? "");
        
        var fileInfo = new System.IO.FileInfo(filePath);
        AddSidebarInfo("📅 Modified", fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"));
        AddSidebarInfo("📅 Created", fileInfo.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"));

        // Action placeholder
        var actionBorder = new System.Windows.Controls.Border
        {
            Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E1E1E")),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(10),
            Margin = new Thickness(0, 15, 0, 0)
        };
        
        var actionText = new System.Windows.Controls.TextBlock
        {
            Text = "File actions will be implemented here (view, edit, copy path, etc.)",
            FontSize = 12,
            Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#888888")),
            TextWrapping = TextWrapping.Wrap
        };
        
        actionBorder.Child = actionText;
        SidebarContent.Children.Add(actionBorder);
    }

    private void UpdateSidebarForFolder(string folderPath, int fileCount, int folderCount)
    {
        SidebarContent.Children.Clear();
        
        // Header
        var header = new System.Windows.Controls.TextBlock
        {
            Text = "FOLDER DETAILS",
            FontSize = 14,
            FontWeight = FontWeights.SemiBold,
            Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#888888")),
            Margin = new Thickness(0, 0, 0, 15)
        };
        SidebarContent.Children.Add(header);

        // Folder info
        AddSidebarInfo("📁 Folder Name", System.IO.Path.GetFileName(folderPath) ?? folderPath);
        AddSidebarInfo("📊 Files", fileCount.ToString("N0"));
        AddSidebarInfo("📂 Subfolders", folderCount.ToString("N0"));
        AddSidebarInfo("📍 Location", folderPath);
        
        var dirInfo = new System.IO.DirectoryInfo(folderPath);
        AddSidebarInfo("📅 Modified", dirInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"));
        AddSidebarInfo("📅 Created", dirInfo.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"));

        // Action placeholder
        var actionBorder = new System.Windows.Controls.Border
        {
            Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E1E1E")),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(10),
            Margin = new Thickness(0, 15, 0, 0)
        };
        
        var actionText = new System.Windows.Controls.TextBlock
        {
            Text = "Folder actions will be implemented here (browse files, search, open in explorer, etc.)",
            FontSize = 12,
            Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#888888")),
            TextWrapping = TextWrapping.Wrap
        };
        
        actionBorder.Child = actionText;
        SidebarContent.Children.Add(actionBorder);
    }

    private void AddSidebarInfo(string label, string value)
    {
        var labelBlock = new System.Windows.Controls.TextBlock
        {
            Text = label,
            FontSize = 12,
            FontWeight = FontWeights.SemiBold,
            Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#4A9EFF")),
            Margin = new Thickness(0, 0, 0, 5)
        };
        SidebarContent.Children.Add(labelBlock);

        var valueBlock = new System.Windows.Controls.TextBlock
        {
            Text = value,
            FontSize = 11,
            Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#CCCCCC")),
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 12)
        };
        SidebarContent.Children.Add(valueBlock);
    }

    private void ResetSidebar()
    {
        SidebarContent.Children.Clear();
        
        var header = new System.Windows.Controls.TextBlock
        {
            Text = "SIDEBAR",
            FontSize = 14,
            FontWeight = FontWeights.SemiBold,
            Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#888888")),
            Margin = new Thickness(0, 0, 0, 15)
        };
        SidebarContent.Children.Add(header);

        var navText = new System.Windows.Controls.TextBlock
        {
            Text = "Navigation",
            FontSize = 13,
            Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#CCCCCC")),
            Margin = new Thickness(0, 0, 0, 10)
        };
        SidebarContent.Children.Add(navText);

        var infoBorder = new System.Windows.Controls.Border
        {
            Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E1E1E")),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(10),
            Margin = new Thickness(0, 0, 0, 10)
        };

        var infoText = new System.Windows.Controls.TextBlock
        {
            Text = "Quick access and tools will appear here based on the selected view.",
            FontSize = 12,
            Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#888888")),
            TextWrapping = TextWrapping.Wrap
        };

        infoBorder.Child = infoText;
        SidebarContent.Children.Add(infoBorder);
    }

    // Store current folder path for refresh functionality
    private string? _currentTreeFolderPath = null;

    private void BuildFileTreeViewWithHeader(string folderPath, string[] extensions, string? selectedFile)
    {
        _currentTreeFolderPath = folderPath;
        SidebarContent.Children.Clear();
        
        // Tree Header: "Files" label with Refresh and Add buttons
        var treeHeader = new System.Windows.Controls.Border
        {
            Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2D2D30")),
            Padding = new Thickness(8, 8, 12, 8),
            Margin = new Thickness(-15, -15, -15, 10),
            BorderBrush = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#3A3A3A")),
            BorderThickness = new Thickness(0, 0, 0, 1)
        };
        
        var headerGrid = new System.Windows.Controls.Grid();
        headerGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        headerGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition { Width = GridLength.Auto });
        headerGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition { Width = GridLength.Auto });
        
        // "Files" label
        var filesLabel = new System.Windows.Controls.TextBlock
        {
            Text = "Files",
            FontSize = 12,
            Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#888888")),
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(4, 0, 0, 0)
        };
        System.Windows.Controls.Grid.SetColumn(filesLabel, 0);
        headerGrid.Children.Add(filesLabel);
        
        // Refresh button
        var refreshBtn = new System.Windows.Controls.Button
        {
            Content = "🔄",
            Background = System.Windows.Media.Brushes.Transparent,
            Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#888888")),
            BorderThickness = new Thickness(0),
            Padding = new Thickness(6, 4, 6, 4),
            Cursor = System.Windows.Input.Cursors.Hand,
            FontSize = 14,
            ToolTip = "Refresh file tree"
        };
        refreshBtn.Click += (s, e) =>
        {
            if (!string.IsNullOrEmpty(_currentTreeFolderPath) && System.IO.Directory.Exists(_currentTreeFolderPath))
            {
                BuildFileTreeViewWithHeader(_currentTreeFolderPath, new[] { ".html", ".htm" }, null);
            }
        };
        System.Windows.Controls.Grid.SetColumn(refreshBtn, 1);
        headerGrid.Children.Add(refreshBtn);
        
        // Add/Open folder button
        var addBtn = new System.Windows.Controls.Button
        {
            Content = "➕",
            Background = System.Windows.Media.Brushes.Transparent,
            Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#888888")),
            BorderThickness = new Thickness(0),
            Padding = new Thickness(6, 4, 6, 4),
            Cursor = System.Windows.Input.Cursors.Hand,
            FontSize = 14,
            ToolTip = "Open a different folder"
        };
        addBtn.Click += (s, e) =>
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Select folder with HTML files",
                ShowNewFolderButton = false
            };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BuildFileTreeViewWithHeader(dialog.SelectedPath, new[] { ".html", ".htm" }, null);
            }
        };
        System.Windows.Controls.Grid.SetColumn(addBtn, 2);
        headerGrid.Children.Add(addBtn);
        
        treeHeader.Child = headerGrid;
        SidebarContent.Children.Add(treeHeader);
        
        // Folder name header
        var folderName = System.IO.Path.GetFileName(folderPath);
        if (string.IsNullOrEmpty(folderName)) folderName = folderPath;
        
        var folderHeader = new System.Windows.Controls.TextBlock
        {
            Text = "📁 " + folderName,
            FontSize = 13,
            FontWeight = FontWeights.SemiBold,
            Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#4A9EFF")),
            Margin = new Thickness(0, 5, 0, 10),
            TextWrapping = TextWrapping.Wrap
        };
        SidebarContent.Children.Add(folderHeader);

        // TreeView for hierarchical folder structure - fills entire sidebar
        var treeView = new System.Windows.Controls.TreeView
        {
            Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent),
            BorderThickness = new Thickness(0),
            Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#CCCCCC"))
        };

        try
        {
            // Get filtered files (.html, .htm)
            var dirInfo = new System.IO.DirectoryInfo(folderPath);
            var allowedExtensions = new[] { ".txt", ".md", ".html", ".htm" };
            var files = dirInfo.GetFiles("*.*", System.IO.SearchOption.AllDirectories)
                .Where(f => allowedExtensions.Contains(f.Extension.ToLower()))
                .ToArray();

            if (files.Length == 0)
            {
                var noFilesText = new System.Windows.Controls.TextBlock
                {
                    Text = "No .txt, .md, or .html files found in this folder.",
                    FontSize = 12,
                    Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#888888")),
                    Margin = new Thickness(0, 10, 0, 0),
                    FontStyle = FontStyles.Italic
                };
                SidebarContent.Children.Add(noFilesText);
            }
            else
            {
                // Build tree structure and add directly to TreeView
                BuildTreeStructure(folderPath, files, selectedFile, treeView);

                // TreeView fills entire sidebar
                SidebarContent.Children.Add(treeView);
            }
        }
        catch (Exception ex)
        {
            var errorText = new System.Windows.Controls.TextBlock
            {
                Text = $"Error loading files: {ex.Message}",
                FontSize = 12,
                Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF6B6B")),
                Margin = new Thickness(0, 10, 0, 0),
                TextWrapping = TextWrapping.Wrap
            };
            SidebarContent.Children.Add(errorText);
        }
    }

    private void BuildFileTreeView(string folderPath, string[] extensions, string? selectedFile)
    {
        SidebarContent.Children.Clear();
        
        // Header with folder name
        var folderName = System.IO.Path.GetFileName(folderPath);
        if (string.IsNullOrEmpty(folderName)) folderName = folderPath;
        
        var header = new System.Windows.Controls.TextBlock
        {
            Text = "📁 " + folderName,
            FontSize = 13,
            FontWeight = FontWeights.SemiBold,
            Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#4A9EFF")),
            Margin = new Thickness(0, 0, 0, 10),
            TextWrapping = TextWrapping.Wrap
        };
        SidebarContent.Children.Add(header);

        // TreeView for hierarchical folder structure - fills entire sidebar
        var treeView = new System.Windows.Controls.TreeView
        {
            Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent),
            BorderThickness = new Thickness(0),
            Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#CCCCCC"))
        };

        try
        {
            // Get filtered files (.txt, .md, .html)
            var dirInfo = new System.IO.DirectoryInfo(folderPath);
            var allowedExtensions = new[] { ".txt", ".md", ".html", ".htm" };
            var files = dirInfo.GetFiles("*.*", System.IO.SearchOption.AllDirectories)
                .Where(f => allowedExtensions.Contains(f.Extension.ToLower()))
                .ToArray();

            if (files.Length == 0)
            {
                var noFilesText = new System.Windows.Controls.TextBlock
                {
                    Text = "No .txt, .md, or .html files found in this folder.",
                    FontSize = 12,
                    Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#888888")),
                    Margin = new Thickness(0, 10, 0, 0),
                    FontStyle = FontStyles.Italic
                };
                SidebarContent.Children.Add(noFilesText);
            }
            else
            {
                // Build tree structure and add directly to TreeView
                BuildTreeStructure(folderPath, files, selectedFile, treeView);

                // TreeView fills entire sidebar
                SidebarContent.Children.Add(treeView);
            }
        }
        catch (Exception ex)
        {
            var errorText = new System.Windows.Controls.TextBlock
            {
                Text = $"Error loading files: {ex.Message}",
                FontSize = 12,
                Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF6B6B")),
                Margin = new Thickness(0, 10, 0, 0),
                TextWrapping = TextWrapping.Wrap
            };
            SidebarContent.Children.Add(errorText);
        }
    }

    private void BuildTreeStructure(string rootPath, System.IO.FileInfo[] files, string? selectedFile, System.Windows.Controls.TreeView treeView)
    {
        var folderMap = new System.Collections.Generic.Dictionary<string, System.Windows.Controls.TreeViewItem>();
        var folderFilesMap = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<System.Windows.Controls.TreeViewItem>>();
        var folderSubfoldersMap = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<System.Windows.Controls.TreeViewItem>>();

        // Group files by their directory and create folder structure
        foreach (var file in files.OrderBy(f => f.Name))
        {
            var fileDir = file.DirectoryName ?? "";
            var relativePath = fileDir.Replace(rootPath, "").TrimStart('\\', '/');
            var pathParts = string.IsNullOrEmpty(relativePath) 
                ? new string[0] 
                : relativePath.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);

            string currentPath = rootPath;

            // Create folder hierarchy
            for (int i = 0; i < pathParts.Length; i++)
            {
                var parentPath = currentPath;
                currentPath = System.IO.Path.Combine(currentPath, pathParts[i]);
                
                if (!folderMap.ContainsKey(currentPath))
                {
                    var folderItem = new System.Windows.Controls.TreeViewItem
                    {
                        Header = $"📂 {pathParts[i]}",
                        IsExpanded = false,
                        Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#CCCCCC")),
                        Tag = currentPath // Store path for sorting
                    };
                    
                    folderMap[currentPath] = folderItem;
                    
                    // Add to parent's subfolder list
                    if (!folderSubfoldersMap.ContainsKey(parentPath))
                    {
                        folderSubfoldersMap[parentPath] = new System.Collections.Generic.List<System.Windows.Controls.TreeViewItem>();
                    }
                    folderSubfoldersMap[parentPath].Add(folderItem);
                }
            }

            // Create file item
            var isSelected = selectedFile != null && file.FullName.Equals(selectedFile, StringComparison.OrdinalIgnoreCase);
            var fileItem = new System.Windows.Controls.TreeViewItem
            {
                Header = $"📄 {file.Name}",
                Tag = file.FullName,
                Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(isSelected ? "#4A9EFF" : "#CCCCCC")),
                Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(isSelected ? "#2D2D30" : "Transparent"))
            };

            fileItem.Selected += (s, e) =>
            {
                e.Handled = true;
                if (fileItem.Tag is string filePath)
                {
                    LoadHtmlFileInWebAnalyzer(filePath);
                }
            };

            // Add file to its directory's file list
            if (!folderFilesMap.ContainsKey(fileDir))
            {
                folderFilesMap[fileDir] = new System.Collections.Generic.List<System.Windows.Controls.TreeViewItem>();
            }
            folderFilesMap[fileDir].Add(fileItem);

            // If this file is selected, expand its parent folders
            if (isSelected)
            {
                ExpandParentFolders(fileDir, rootPath, folderMap);
            }
        }

        // Build tree: add files first, then subfolders to each folder
        void AddItemsToFolder(string folderPath, System.Windows.Controls.ItemCollection items)
        {
            // First add all files in this folder (already sorted by name)
            if (folderFilesMap.ContainsKey(folderPath))
            {
                foreach (var fileItem in folderFilesMap[folderPath].OrderBy(f => ((string)f.Header).Substring(2))) // Remove emoji for sorting
                {
                    items.Add(fileItem);
                }
            }

            // Then add all subfolders (sorted by name)
            if (folderSubfoldersMap.ContainsKey(folderPath))
            {
                foreach (var subfolder in folderSubfoldersMap[folderPath].OrderBy(f => ((string)f.Header).Substring(2)))
                {
                    items.Add(subfolder);
                    // Recursively add items to this subfolder
                    if (subfolder.Tag is string subfolderPath)
                    {
                        AddItemsToFolder(subfolderPath, subfolder.Items);
                    }
                }
            }
        }

        // Start with root folder
        AddItemsToFolder(rootPath, treeView.Items);
    }

    private void ExpandParentFolders(string fileDir, string rootPath, System.Collections.Generic.Dictionary<string, System.Windows.Controls.TreeViewItem> folderMap)
    {
        var relativePath = fileDir.Replace(rootPath, "").TrimStart('\\', '/');
        if (string.IsNullOrEmpty(relativePath)) return;

        var pathParts = relativePath.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
        string currentPath = rootPath;

        foreach (var part in pathParts)
        {
            currentPath = System.IO.Path.Combine(currentPath, part);
            if (folderMap.ContainsKey(currentPath))
            {
                folderMap[currentPath].IsExpanded = true;
            }
        }
    }

    // Store currently selected file info for future use
    private string? _selectedFilePath = null;
    private string? _selectedFileName = null;

    private void LoadHtmlFileInWebAnalyzer(string filePath)
    {
        try
        {
            // Store selected file info (will be used in future features)
            _selectedFilePath = filePath;
            _selectedFileName = System.IO.Path.GetFileName(filePath);

            // Update the header to show selected file
            UpdateHeader("📄", _selectedFileName, _selectedFilePath);

            // No message box - just silently remember the selection
            // Future features will use _selectedFilePath and _selectedFileName
        }
        catch (Exception ex)
        {
            // Only show error if something went wrong
            System.Windows.MessageBox.Show(
                $"Error selecting file: {ex.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    #endregion
}
