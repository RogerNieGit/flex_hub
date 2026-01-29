using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ModernDesktopApp;

/// <summary>
/// AI Analysis Service using Fuelix AI (OpenAI-compatible API)
/// </summary>
public class AiAnalysisService
{
    private readonly HttpClient _httpClient;
    private readonly AiKeyManager _keyManager;
    private string _apiKey = "";
    private string _baseUrl = "https://proxy.fuelix.ai/v1";
    private string _model = "gpt-4o-mini"; // Default model

    public AiAnalysisService()
    {
        _httpClient = new HttpClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(60);
        _keyManager = new AiKeyManager();
        
        // Try to load from database
        LoadFromDatabase();
    }

    /// <summary>
    /// Load API key and settings from database
    /// </summary>
    private void LoadFromDatabase()
    {
        try
        {
            var keyInfo = _keyManager.GetApiKey("fuelix");
            if (keyInfo != null)
            {
                _apiKey = keyInfo.ApiKey;
                _baseUrl = keyInfo.BaseUrl;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load API key from database: {ex.Message}");
        }
    }

    public void Configure(string apiKey, string? baseUrl = null, string? model = null)
    {
        _apiKey = apiKey;
        if (!string.IsNullOrEmpty(baseUrl))
            _baseUrl = baseUrl;
        if (!string.IsNullOrEmpty(model))
            _model = model;
    }

    /// <summary>
    /// Save current configuration to database
    /// </summary>
    public bool SaveToDatabase(string provider = "fuelix")
    {
        return _keyManager.SaveApiKey(provider, _apiKey, _baseUrl);
    }

    /// <summary>
    /// Get available models from database
    /// </summary>
    public List<AiModel> GetAvailableModels()
    {
        return _keyManager.GetAvailableModels();
    }

    public bool IsConfigured => !string.IsNullOrEmpty(_apiKey);

    public async Task<string> AnalyzeHtml(string htmlContent, string userQuery)
    {
        if (!IsConfigured)
        {
            throw new InvalidOperationException("AI service not configured. Please set your API key first.");
        }

        try
        {
            var systemPrompt = @"You are an expert HTML/web content analyst. You help users analyze, understand, and extract information from HTML source code. 

Your capabilities include:
1. Analyzing HTML structure and identifying key elements
2. Extracting specific data (prices, emails, links, etc.)
3. Summarizing page content
4. Finding patterns and providing insights
5. Explaining technical aspects in clear language

When extracting data, format your response clearly with headings and bullet points.
When asked to extract specific information, provide it in a structured format that's easy to copy or export.";

            var userPrompt = $@"User Query: {userQuery}

HTML Content (first 8000 chars):
{(htmlContent.Length > 8000 ? htmlContent.Substring(0, 8000) + "\n...[truncated]" : htmlContent)}

Please analyze the HTML and respond to the user's query.";

            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                temperature = 0.7,
                max_tokens = 2000
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var response = await _httpClient.PostAsync($"{_baseUrl}/chat/completions", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return $"Error: API request failed with status {response.StatusCode}\n{responseContent}";
            }

            var jsonResponse = JsonDocument.Parse(responseContent);
            var assistantMessage = jsonResponse.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return assistantMessage ?? "No response from AI";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}\n\nPlease check your API configuration and internet connection.";
        }
    }

    public async Task<string> QuickAnalysis(string htmlContent, AnalysisType type)
    {
        var queries = new Dictionary<AnalysisType, string>
        {
            { AnalysisType.Summary, "Provide a brief summary of what this webpage is about based on the HTML content." },
            { AnalysisType.ExtractLinks, "Extract all links (href attributes) from this HTML. List them with their anchor text if available." },
            { AnalysisType.ExtractImages, "List all images (img src) found in this HTML with their alt text if available." },
            { AnalysisType.ExtractForms, "Identify and describe all forms in this HTML, including input fields and their purposes." },
            { AnalysisType.ExtractScripts, "List all script sources (src attributes) and describe what external scripts are loaded." },
            { AnalysisType.SEOAnalysis, "Analyze this page's SEO: check for title, meta description, headings structure, and provide recommendations." },
            { AnalysisType.StructureAnalysis, "Analyze the HTML structure: main sections, semantic HTML usage, and overall organization." },
            { AnalysisType.ExtractContact, "Find and extract any contact information: emails, phone numbers, addresses, social media links." },
            { AnalysisType.ExtractPrices, "Find and extract any prices, costs, or monetary values mentioned in the HTML." },
            { AnalysisType.AccessibilityCheck, "Review this HTML for accessibility issues: alt texts, ARIA labels, semantic structure, etc." }
        };

        return await AnalyzeHtml(htmlContent, queries[type]);
    }
}

public enum AnalysisType
{
    Summary,
    ExtractLinks,
    ExtractImages,
    ExtractForms,
    ExtractScripts,
    SEOAnalysis,
    StructureAnalysis,
    ExtractContact,
    ExtractPrices,
    AccessibilityCheck
}