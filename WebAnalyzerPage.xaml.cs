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

    public WebAnalyzerPage()
    {
        InitializeComponent();
        _aiService = new AiAnalysisService();
        InitializeWebView();
    }

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
