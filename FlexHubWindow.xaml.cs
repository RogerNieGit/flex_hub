using System;
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
            DragMove();
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
            Filter = "All Files (*.*)|*.*|Text Files (*.txt)|*.txt|C# Files (*.cs)|*.cs|JSON Files (*.json)|*.json|XML Files (*.xml)|*.xml"
        };

        if (dialog.ShowDialog() == true)
        {
            string fileName = System.IO.Path.GetFileName(dialog.FileName);
            string fileSize = new System.IO.FileInfo(dialog.FileName).Length.ToString("N0");
            string fileExt = System.IO.Path.GetExtension(dialog.FileName);
            
            UpdateHeader("📄", fileName, dialog.FileName);
            UpdateSidebarForFile(dialog.FileName, fileSize, fileExt);
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
            string folderName = System.IO.Path.GetFileName(dialog.SelectedPath);
            var dirInfo = new System.IO.DirectoryInfo(dialog.SelectedPath);
            int fileCount = dirInfo.GetFiles("*", System.IO.SearchOption.AllDirectories).Length;
            int folderCount = dirInfo.GetDirectories("*", System.IO.SearchOption.AllDirectories).Length;
            
            UpdateHeader("📁", folderName, dialog.SelectedPath);
            UpdateSidebarForFolder(dialog.SelectedPath, fileCount, folderCount);
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
        ResetSidebar();
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

    #endregion
}
