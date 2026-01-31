using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Web.WebView2.Core;
using Microsoft.Win32;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Key = System.Windows.Input.Key;

namespace ModernDesktopApp;

/// <summary>
/// Web Analyzer Page with Microsoft Edge WebView2 and AI Analysis
/// </summary>
public partial class WebAnalyzerPage : Page
{
    private readonly AiAnalysisService _aiService;
    private string _lastAiResponse = "";
    
    // Settings key for default folder
    private const string DefaultFolderSettingKey = "WebAnalyzer_DefaultFolder";
    
    // Event to notify parent window to update sidebar with tree
    public event Action<string>? OnDefaultFolderLoaded;

    public WebAnalyzerPage()
    {
        InitializeComponent();
        _aiService = new AiAnalysisService();
        InitializeWebView();
        
        // Load default folder on startup
        Loaded += WebAnalyzerPage_Loaded;
    }

    private void WebAnalyzerPage_Loaded(object sender, RoutedEventArgs e)
    {
        LoadDefaultFolderIfValid();
    }

    private void LoadDefaultFolderIfValid()
    {
        var defaultFolder = GetDefaultFolder();
        if (!string.IsNullOrEmpty(defaultFolder) && Directory.Exists(defaultFolder))
        {
            // Notify parent window to build tree view
            OnDefaultFolderLoaded?.Invoke(defaultFolder);
        }
    }
    
    #region Settings Management
    
    public static string? GetDefaultFolder()
    {
        try
        {
            return Microsoft.Win32.Registry.CurrentUser
                .OpenSubKey("SOFTWARE\\FlexHub")?
                .GetValue(DefaultFolderSettingKey) as string;
        }
        catch
        {
            return null;
        }
    }
    
    public static void SaveDefaultFolder(string folderPath)
    {
        try
        {
            var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\FlexHub");
            key?.SetValue(DefaultFolderSettingKey, folderPath);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                $"Error saving settings: {ex.Message}",
                "Settings Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
    
    public static void ClearDefaultFolder()
    {
        try
        {
            var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\FlexHub", true);
            key?.DeleteValue(DefaultFolderSettingKey, false);
        }
        catch
        {
            // Ignore if value doesn't exist
        }
    }
    
    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        ShowSettingsDialog();
    }
    
    private void ShowSettingsDialog()
    {
        var dialog = new Window
        {
            Title = "Web Analyzer Settings",
            Width = 500,
            Height = 280,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = Window.GetWindow(this),
            Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2D2D30")),
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
            Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#CCCCCC")),
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
            Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#4A9EFF")),
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
            Text = GetDefaultFolder() ?? "",
            Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E1E1E")),
            Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#CCCCCC")),
            BorderBrush = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#3C3C3C")),
            Padding = new Thickness(10, 8, 10, 8),
            FontSize = 12,
            VerticalContentAlignment = VerticalAlignment.Center
        };
        System.Windows.Controls.Grid.SetColumn(folderTextBox, 0);
        inputGrid.Children.Add(folderTextBox);
        
        var browseBtn = new System.Windows.Controls.Button
        {
            Content = "📁 Browse",
            Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#3A3A3A")),
            Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#CCCCCC")),
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
            Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#3A3A3A")),
            Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#CCCCCC")),
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
            Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#888888")),
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
            Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#3A3A3A")),
            Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#CCCCCC")),
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
            Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#4A9EFF")),
            Foreground = new SolidColorBrush(Colors.White),
            BorderThickness = new Thickness(0),
            Padding = new Thickness(12, 8, 12, 8),
            Cursor = System.Windows.Input.Cursors.Hand
        };
        saveBtn.Click += (s, ev) =>
        {
            var folderPath = folderTextBox.Text.Trim();
            if (string.IsNullOrEmpty(folderPath))
            {
                ClearDefaultFolder();
                System.Windows.MessageBox.Show("Default folder cleared.", "Settings Saved", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (Directory.Exists(folderPath))
            {
                SaveDefaultFolder(folderPath);
                System.Windows.MessageBox.Show("✅ Settings saved!", "Settings Saved", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                // Notify parent to load tree
                OnDefaultFolderLoaded?.Invoke(folderPath);
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
    
    #endregion

    private async void InitializeWebView()
    {
        try
        {
            await WebBrowser.EnsureCoreWebView2Async(null);
            
            // Subscribe to navigation events
            WebBrowser.NavigationStarting += WebBrowser_NavigationStarting;
            WebBrowser.NavigationCompleted += WebBrowser_NavigationCompleted;
            WebBrowser.SourceChanged += WebBrowser_SourceChanged;
            
            // Initial navigation
            WebBrowser.Source = new Uri("https://www.google.com");
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                $"Failed to initialize WebView2: {ex.Message}\n\n" +
                "Please ensure Microsoft Edge WebView2 Runtime is installed.",
                "WebView2 Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    #region Navigation Events

    private void WebBrowser_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        LoadingIndicator.Visibility = Visibility.Visible;
    }

    private void WebBrowser_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        LoadingIndicator.Visibility = Visibility.Collapsed;
        
        if (e.IsSuccess)
        {
            UpdateSourceView();
        }
    }

    private void WebBrowser_SourceChanged(object? sender, CoreWebView2SourceChangedEventArgs e)
    {
        UrlTextBox.Text = WebBrowser.Source?.ToString() ?? "";
    }

    #endregion

    #region Button Handlers

    private void Go_Click(object sender, RoutedEventArgs e)
    {
        NavigateToUrl();
    }

    private void UrlTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            NavigateToUrl();
        }
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        if (WebBrowser.CanGoBack)
        {
            WebBrowser.GoBack();
        }
    }

    private void Forward_Click(object sender, RoutedEventArgs e)
    {
        if (WebBrowser.CanGoForward)
        {
            WebBrowser.GoForward();
        }
    }

    private void Refresh_Click(object sender, RoutedEventArgs e)
    {
        WebBrowser.Reload();
    }

    #endregion

    #region Search Functionality

    private List<int> _searchMatches = new List<int>();
    private int _currentMatchIndex = -1;

    private void SearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        PerformSearch();
    }

    private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Shift)
            {
                FindPrevious();
            }
            else
            {
                FindNext();
            }
        }
        else if (e.Key == Key.Escape)
        {
            SearchTextBox.Clear();
            SourceTextBox.Select(0, 0);
        }
    }

    private void FindNext_Click(object sender, RoutedEventArgs e)
    {
        FindNext();
    }

    private void FindPrevious_Click(object sender, RoutedEventArgs e)
    {
        FindPrevious();
    }

    private void PerformSearch()
    {
        _searchMatches.Clear();
        _currentMatchIndex = -1;
        
        string searchText = SearchTextBox.Text;
        string sourceText = SourceTextBox.Text;

        if (string.IsNullOrEmpty(searchText) || string.IsNullOrEmpty(sourceText))
        {
            SearchResultsText.Text = "";
            SourceTextBox.Select(0, 0);
            return;
        }

        // Find all matches (case-insensitive)
        int index = 0;
        while ((index = sourceText.IndexOf(searchText, index, StringComparison.OrdinalIgnoreCase)) != -1)
        {
            _searchMatches.Add(index);
            index += searchText.Length;
        }

        if (_searchMatches.Count > 0)
        {
            _currentMatchIndex = 0;
            HighlightMatch();
            UpdateSearchResults();
        }
        else
        {
            SearchResultsText.Text = "No matches";
            SourceTextBox.Select(0, 0);
        }
    }

    private void FindNext()
    {
        if (_searchMatches.Count == 0)
        {
            PerformSearch();
            return;
        }

        _currentMatchIndex = (_currentMatchIndex + 1) % _searchMatches.Count;
        HighlightMatch();
        UpdateSearchResults();
    }

    private void FindPrevious()
    {
        if (_searchMatches.Count == 0)
        {
            PerformSearch();
            return;
        }

        _currentMatchIndex--;
        if (_currentMatchIndex < 0)
        {
            _currentMatchIndex = _searchMatches.Count - 1;
        }
        
        HighlightMatch();
        UpdateSearchResults();
    }

    private void HighlightMatch()
    {
        if (_currentMatchIndex >= 0 && _currentMatchIndex < _searchMatches.Count)
        {
            int position = _searchMatches[_currentMatchIndex];
            int length = SearchTextBox.Text.Length;

            SourceTextBox.Focus();
            SourceTextBox.Select(position, length);
            SourceTextBox.ScrollToLine(SourceTextBox.GetLineIndexFromCharacterIndex(position));
        }
    }

    private void UpdateSearchResults()
    {
        if (_searchMatches.Count > 0)
        {
            SearchResultsText.Text = $"{_currentMatchIndex + 1} of {_searchMatches.Count}";
        }
        else
        {
            SearchResultsText.Text = "No matches";
        }
    }

    #endregion

    #region Helper Methods

    private void NavigateToUrl()
    {
        string url = UrlTextBox.Text.Trim();
        
        if (string.IsNullOrEmpty(url))
        {
            System.Windows.MessageBox.Show(
                "Please enter a valid URL.",
                "Invalid URL",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        // Add https:// if no protocol specified
        if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            url = "https://" + url;
        }

        try
        {
            WebBrowser.Source = new Uri(url);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                $"Failed to navigate to URL: {ex.Message}",
                "Navigation Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void UpdateSourceView()
    {
        try
        {
            if (WebBrowser.CoreWebView2 != null)
            {
                string html = await WebBrowser.CoreWebView2.ExecuteScriptAsync(
                    "document.documentElement.outerHTML");
                
                // Remove quotes and unescape
                html = System.Text.Json.JsonSerializer.Deserialize<string>(html) ?? "";
                
                SourceTextBox.Text = FormatHtml(html);
            }
        }
        catch (Exception ex)
        {
            SourceTextBox.Text = $"Error retrieving source: {ex.Message}";
        }
    }

    private string FormatHtml(string html)
    {
        // Basic HTML formatting for better readability
        try
        {
            // Add line breaks after closing tags
            html = System.Text.RegularExpressions.Regex.Replace(html, @"(>)(<)", "$1\n$2");
            
            return html;
        }
        catch
        {
            return html;
        }
    }

    #endregion

    #region Word Wrap

    private void WordWrap_Changed(object sender, RoutedEventArgs e)
    {
        if (SourceTextBox != null)
        {
            SourceTextBox.TextWrapping = WordWrapCheckBox.IsChecked == true 
                ? TextWrapping.Wrap 
                : TextWrapping.NoWrap;
        }
    }

    #endregion

    #region Save Source

    private void SaveSource_Click(object sender, RoutedEventArgs e)
    {
        var htmlContent = SourceTextBox.Text;
        
        if (string.IsNullOrEmpty(htmlContent))
        {
            System.Windows.MessageBox.Show(
                "No HTML content available to save. Please navigate to a webpage first.",
                "No Content",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            Title = "Save HTML Source",
            Filter = "HTML File (*.html)|*.html|Text File (*.txt)|*.txt|All Files (*.*)|*.*",
            DefaultExt = ".html",
            FileName = $"source_{DateTime.Now:yyyyMMdd_HHmmss}.html"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                File.WriteAllText(dialog.FileName, htmlContent);
                
                System.Windows.MessageBox.Show(
                    $"HTML source saved successfully to:\n{dialog.FileName}",
                    "Save Successful",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Failed to save file: {ex.Message}",
                    "Save Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }

    #endregion

    #region AI Analysis

    private void AiSettings_Click(object sender, RoutedEventArgs e)
    {
        var settingsDialog = new AiSettingsDialog(_aiService);
        settingsDialog.Owner = Window.GetWindow(this);
        
        if (settingsDialog.ShowDialog() == true)
        {
            System.Windows.MessageBox.Show(
                "AI settings saved successfully!\nYou can now use AI analysis features.",
                "Settings Saved",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }

    private async void QuickAnalyze_Click(object sender, RoutedEventArgs e)
    {
        if (!_aiService.IsConfigured)
        {
            var result = System.Windows.MessageBox.Show(
                "AI service is not configured. Would you like to configure it now?",
                "AI Not Configured",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                AiSettings_Click(sender, e);
            }
            return;
        }

        var selectedItem = QuickAnalysisComboBox.SelectedItem as ComboBoxItem;
        if (selectedItem == null)
        {
            System.Windows.MessageBox.Show(
                "Please select an analysis type.",
                "No Selection",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        var analysisType = Enum.Parse<AnalysisType>(selectedItem.Tag.ToString() ?? "Summary");
        var htmlContent = SourceTextBox.Text;

        if (string.IsNullOrEmpty(htmlContent))
        {
            System.Windows.MessageBox.Show(
                "No HTML content available. Please navigate to a webpage first.",
                "No Content",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        await PerformAiAnalysis(analysisType, htmlContent);
    }

    private async void AskAi_Click(object sender, RoutedEventArgs e)
    {
        var query = CustomQueryTextBox.Text.Trim();
        
        if (query == "Ask a question about the HTML..." || string.IsNullOrEmpty(query))
        {
            System.Windows.MessageBox.Show(
                "Please enter a question.",
                "No Question",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        if (!_aiService.IsConfigured)
        {
            var result = System.Windows.MessageBox.Show(
                "AI service is not configured. Would you like to configure it now?",
                "AI Not Configured",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                AiSettings_Click(sender, e);
            }
            return;
        }

        var htmlContent = SourceTextBox.Text;
        
        if (string.IsNullOrEmpty(htmlContent))
        {
            System.Windows.MessageBox.Show(
                "No HTML content available. Please navigate to a webpage first.",
                "No Content",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        await PerformCustomAiQuery(query, htmlContent);
    }

    private void CustomQuery_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            AskAi_Click(sender, e);
        }
    }

    private async System.Threading.Tasks.Task PerformAiAnalysis(AnalysisType analysisType, string htmlContent)
    {
        // Add user query message
        AddChatMessage($"📊 {analysisType}", true);
        
        // Add "thinking" message
        var thinkingBorder = AddChatMessage("🤔 Analyzing...", false);
        
        try
        {
            var response = await _aiService.QuickAnalysis(htmlContent, analysisType);
            _lastAiResponse = response;
            
            // Remove thinking message
            AiChatPanel.Children.Remove(thinkingBorder);
            
            // Add AI response
            AddChatMessage(response, false);
        }
        catch (Exception ex)
        {
            AiChatPanel.Children.Remove(thinkingBorder);
            AddChatMessage($"❌ Error: {ex.Message}", false);
        }
    }

    private async System.Threading.Tasks.Task PerformCustomAiQuery(string query, string htmlContent)
    {
        // Add user query message
        AddChatMessage(query, true);
        
        // Clear input
        CustomQueryTextBox.Text = "Ask a question about the HTML...";
        
        // Add "thinking" message
        var thinkingBorder = AddChatMessage("🤔 Analyzing...", false);
        
        try
        {
            var response = await _aiService.AnalyzeHtml(htmlContent, query);
            _lastAiResponse = response;
            
            // Remove thinking message
            AiChatPanel.Children.Remove(thinkingBorder);
            
            // Add AI response
            AddChatMessage(response, false);
        }
        catch (Exception ex)
        {
            AiChatPanel.Children.Remove(thinkingBorder);
            AddChatMessage($"❌ Error: {ex.Message}", false);
        }
    }

    private Border AddChatMessage(string message, bool isUser)
    {
        var border = new Border
        {
            Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(isUser ? "#2D2D30" : "#252525")),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(15),
            Margin = new Thickness(0, 0, 0, 10),
            HorizontalAlignment = isUser ? System.Windows.HorizontalAlignment.Right : System.Windows.HorizontalAlignment.Stretch,
            MaxWidth = isUser ? 600 : double.PositiveInfinity
        };

        var textBlock = new TextBlock
        {
            Text = message,
            Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#CCCCCC")),
            FontSize = 13,
            TextWrapping = TextWrapping.Wrap,
            FontFamily = message.StartsWith("❌") || message.StartsWith("🤔") ? new System.Windows.Media.FontFamily("Segoe UI") : new System.Windows.Media.FontFamily("Segoe UI")
        };

        if (!isUser && !message.StartsWith("❌") && !message.StartsWith("🤔"))
        {
            textBlock.FontFamily = new System.Windows.Media.FontFamily("Consolas");
            textBlock.FontSize = 12;
        }

        border.Child = textBlock;
        AiChatPanel.Children.Add(border);
        
        return border;
    }

    private void Export_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_lastAiResponse))
        {
            System.Windows.MessageBox.Show(
                "No AI analysis result to export. Please run an analysis first.",
                "Nothing to Export",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            Title = "Export AI Analysis Result",
            Filter = "Text File (*.txt)|*.txt|Markdown File (*.md)|*.md|JSON File (*.json)|*.json|All Files (*.*)|*.*",
            DefaultExt = ".txt",
            FileName = $"ai_analysis_{DateTime.Now:yyyyMMdd_HHmmss}"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                File.WriteAllText(dialog.FileName, _lastAiResponse);
                
                System.Windows.MessageBox.Show(
                    $"Analysis exported successfully to:\n{dialog.FileName}",
                    "Export Successful",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Failed to export: {ex.Message}",
                    "Export Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }

    #endregion
}
