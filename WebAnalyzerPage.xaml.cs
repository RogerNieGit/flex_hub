using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Key = System.Windows.Input.Key;

namespace ModernDesktopApp;

/// <summary>
/// Web Analyzer Page with Microsoft Edge WebView2
/// </summary>
public partial class WebAnalyzerPage : Page
{
    public WebAnalyzerPage()
    {
        InitializeComponent();
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
}