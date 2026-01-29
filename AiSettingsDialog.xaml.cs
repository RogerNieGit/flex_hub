using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MessageBox = System.Windows.MessageBox;

namespace ModernDesktopApp;

/// <summary>
/// AI Settings Dialog for configuring Fuelix AI with database support
/// </summary>
public partial class AiSettingsDialog : Window
{
    private readonly AiAnalysisService _aiService;
    private readonly AiKeyManager _keyManager;
    
    public string ApiKey { get; private set; } = "";
    public string BaseUrl { get; private set; } = "";
    public string Model { get; private set; } = "";

    public AiSettingsDialog(AiAnalysisService aiService)
    {
        InitializeComponent();
        _aiService = aiService;
        _keyManager = new AiKeyManager();
        
        // Load models dynamically from database
        LoadAvailableModels();
        
        // Load existing settings from database
        LoadSettings();
    }

    private void LoadAvailableModels()
    {
        try
        {
            var models = _aiService.GetAvailableModels();
            
            ModelComboBox.Items.Clear();
            
            foreach (var model in models)
            {
                var item = new ComboBoxItem
                {
                    Content = model.Name,
                    Tag = model.Name
                };
                ModelComboBox.Items.Add(item);
            }
            
            // Select first model as default
            if (ModelComboBox.Items.Count > 0)
            {
                ModelComboBox.SelectedIndex = 0;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Failed to load models from database: {ex.Message}\nUsing default models.",
                "Warning",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            
            // Fallback to default models
            LoadDefaultModels();
        }
    }

    private void LoadDefaultModels()
    {
        ModelComboBox.Items.Clear();
        ModelComboBox.Items.Add(new ComboBoxItem { Content = "gpt-4o-mini", Tag = "gpt-4o-mini" });
        ModelComboBox.Items.Add(new ComboBoxItem { Content = "gpt-4o", Tag = "gpt-4o" });
        ModelComboBox.Items.Add(new ComboBoxItem { Content = "gpt-3.5-turbo", Tag = "gpt-3.5-turbo" });
        ModelComboBox.Items.Add(new ComboBoxItem { Content = "claude-3-5-sonnet-20241022", Tag = "claude-3-5-sonnet-20241022" });
        ModelComboBox.SelectedIndex = 0;
    }

    private void LoadSettings()
    {
        try
        {
            // Load from database
            var keyInfo = _keyManager.GetApiKey("fuelix");
            if (keyInfo != null)
            {
                // Note: We can't show the decrypted key in PasswordBox for security
                // User will need to re-enter or we can show a placeholder
                BaseUrlBox.Text = keyInfo.BaseUrl;
                
                // Show indicator that key exists
                ApiKeyBox.Password = ""; // Leave empty for security
                
                MessageBox.Show(
                    "Existing API key found in database (encrypted).\nYou can test the connection or enter a new key to update.",
                    "API Key Loaded",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                // No key found, use defaults
                BaseUrlBox.Text = "https://proxy.fuelix.ai/v1";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Failed to load settings from database: {ex.Message}",
                "Warning",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            
            // Use defaults
            BaseUrlBox.Text = "https://proxy.fuelix.ai/v1";
        }
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to open link: {ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void TestConnection_Click(object sender, RoutedEventArgs e)
    {
        var apiKey = ApiKeyBox.Password;
        var baseUrl = BaseUrlBox.Text.Trim();
        var model = (ModelComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();

        if (string.IsNullOrEmpty(apiKey))
        {
            MessageBox.Show("Please enter your API key.", "Validation Error",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            // Configure temporary service for testing
            var testService = new AiAnalysisService();
            testService.Configure(apiKey, baseUrl, model);

            // Show loading state
            var testButton = sender as System.Windows.Controls.Button;
            if (testButton != null)
            {
                testButton.Content = "Testing...";
                testButton.IsEnabled = false;
            }

            // Test with simple HTML
            var testHtml = "<html><head><title>Test</title></head><body><h1>Hello World</h1></body></html>";
            var result = await testService.AnalyzeHtml(testHtml, "What is this page about? Just say 'Test successful' if you can read this.");

            // Restore button
            if (testButton != null)
            {
                testButton.Content = "Test Connection";
                testButton.IsEnabled = true;
            }

            if (result.Contains("Error:"))
            {
                MessageBox.Show($"Connection test failed:\n\n{result}", "Test Failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show($"✅ Connection successful!\n\nAI Response: {result}", "Test Successful",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Connection test failed:\n\n{ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        var apiKey = ApiKeyBox.Password;
        var baseUrl = BaseUrlBox.Text.Trim();
        var model = (ModelComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();

        // If API key is empty, try to use existing key from database
        if (string.IsNullOrEmpty(apiKey))
        {
            var existingKey = _keyManager.GetApiKey("fuelix");
            if (existingKey != null && !string.IsNullOrEmpty(existingKey.ApiKey))
            {
                apiKey = existingKey.ApiKey;
            }
            else
            {
                MessageBox.Show("Please enter your API key.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        if (string.IsNullOrEmpty(baseUrl))
        {
            MessageBox.Show("Please enter the base URL.", "Validation Error",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Save settings
        ApiKey = apiKey;
        BaseUrl = baseUrl;
        Model = model ?? "gpt-4o-mini";

        // Configure the service
        _aiService.Configure(ApiKey, BaseUrl, Model);

        // Save to database with encryption
        try
        {
            bool saved = _aiService.SaveToDatabase("fuelix");
            if (saved)
            {
                MessageBox.Show(
                    "✅ API key saved successfully to database (encrypted).",
                    "Settings Saved",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(
                    "⚠️ Settings configured but failed to save to database.\nKey will work for this session only.",
                    "Warning",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"⚠️ Settings configured but database save failed: {ex.Message}\nKey will work for this session only.",
                "Warning",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}