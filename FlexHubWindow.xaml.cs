using System;
using System.Linq;
using System.Windows;
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
                // For Web Analyzer, show tree view of HTML files
                BuildFileTreeView(dialog.SelectedPath, new[] { ".html", ".htm" }, null);
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
    }

    private void WebAnalyzer_Click(object sender, RoutedEventArgs e)
    {
        UpdateHeader("🌐", "Web Analyzer", "Browse and analyze web pages");
        ContentFrame.Navigate(new WebAnalyzerPage());
        UpdateWebAnalyzerSidebar();
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
        UpdateHeader("📝", "Quick Book", "Note-taking and documentation");
        // TODO: Load quick book page
    }

    private void Analytics_Click(object sender, RoutedEventArgs e)
    {
        UpdateHeader("📊", "Analytics", "Data visualization and insights");
        // TODO: Load analytics page
    }

    private void Profile_Click(object sender, RoutedEventArgs e)
    {
        UpdateHeader("👤", "Profile", "User settings and preferences");
        // TODO: Load profile page
    }

    private void Settings_Click(object sender, RoutedEventArgs e)
    {
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

        // ScrollViewer for file list
        var scrollViewer = new System.Windows.Controls.ScrollViewer
        {
            VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto,
            MaxHeight = 600
        };

        var fileStack = new System.Windows.Controls.StackPanel();

        try
        {
            // Get all files matching the extensions
            var dirInfo = new System.IO.DirectoryInfo(folderPath);
            var files = dirInfo.GetFiles("*.*", System.IO.SearchOption.AllDirectories)
                .Where(f => extensions.Any(ext => f.Extension.Equals(ext, StringComparison.OrdinalIgnoreCase)))
                .OrderBy(f => f.FullName)
                .ToArray();

            if (files.Length == 0)
            {
                var noFilesText = new System.Windows.Controls.TextBlock
                {
                    Text = "No HTML files found in this folder.",
                    FontSize = 12,
                    Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#888888")),
                    Margin = new Thickness(0, 10, 0, 0),
                    FontStyle = FontStyles.Italic
                };
                fileStack.Children.Add(noFilesText);
            }
            else
            {
                foreach (var file in files)
                {
                    var isSelected = selectedFile != null && file.FullName.Equals(selectedFile, StringComparison.OrdinalIgnoreCase);
                    
                    var fileButton = new System.Windows.Controls.Button
                    {
                        Content = "📄 " + file.Name,
                        Tag = file.FullName,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                        HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left,
                        Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(isSelected ? "#2D2D30" : "Transparent")),
                        Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(isSelected ? "#4A9EFF" : "#CCCCCC")),
                        BorderThickness = new Thickness(0),
                        Padding = new Thickness(5, 8, 5, 8),
                        FontSize = 12,
                        Cursor = System.Windows.Input.Cursors.Hand,
                        Margin = new Thickness(0, 2, 0, 2)
                    };

                    fileButton.Click += (s, e) =>
                    {
                        var btn = s as System.Windows.Controls.Button;
                        if (btn?.Tag is string filePath)
                        {
                            LoadHtmlFileInWebAnalyzer(filePath);
                        }
                    };

                    // Hover effect
                    fileButton.MouseEnter += (s, e) =>
                    {
                        if (fileButton.Background is System.Windows.Media.SolidColorBrush brush && brush.Color == System.Windows.Media.Colors.Transparent)
                        {
                            fileButton.Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#3C3C3C"));
                        }
                    };

                    fileButton.MouseLeave += (s, e) =>
                    {
                        if (!isSelected)
                        {
                            fileButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
                        }
                    };

                    fileStack.Children.Add(fileButton);
                }

                // Add file count info
                var infoText = new System.Windows.Controls.TextBlock
                {
                    Text = $"\n{files.Length} HTML file(s) found",
                    FontSize = 11,
                    Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#888888")),
                    Margin = new Thickness(0, 10, 0, 0),
                    FontStyle = FontStyles.Italic
                };
                fileStack.Children.Add(infoText);
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
            fileStack.Children.Add(errorText);
        }

        scrollViewer.Content = fileStack;
        SidebarContent.Children.Add(scrollViewer);
    }

    private void LoadHtmlFileInWebAnalyzer(string filePath)
    {
        try
        {
            // Update the header
            string fileName = System.IO.Path.GetFileName(filePath);
            UpdateHeader("📄", fileName, filePath);

            // Get the WebAnalyzerPage and navigate to the file
            if (ContentFrame.Content is WebAnalyzerPage webAnalyzer)
            {
                // Load the HTML file as a local file URI
                var fileUri = new Uri(filePath);
                // We'll need to expose a method in WebAnalyzerPage to load a file
                // For now, just show a message
                System.Windows.MessageBox.Show(
                    $"File selected: {fileName}\n\nTo view this file, WebAnalyzerPage needs a LoadFile method implemented.",
                    "File Selected",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                $"Error loading file: {ex.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    #endregion
}
