using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using JulyAgent.Constants;
using JulyAgent.Interfaces;
using JulyAgent.Models;

namespace JulyAgent.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GeminiService> _logger;
        private readonly ISettingsService _settingsService;

        public GeminiService(HttpClient httpClient, ILogger<GeminiService> logger, ISettingsService settingsService)
        {
            _httpClient = httpClient;
            _logger = logger;
            _settingsService = settingsService;
            
            // Configure default headers
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(ApiConstants.UserAgentHeader);
            _httpClient.Timeout = TimeSpan.FromSeconds(ApiConstants.DefaultTimeoutSeconds);
        }

        public async Task<string> GenerateContentAsync(string prompt, string model, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await GenerateContentDetailedAsync(prompt, model, cancellationToken);
                return ExtractTextFromResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating content with model {Model}", model);
                throw;
            }
        }

        public async Task<GeminiResponse> GenerateContentDetailedAsync(string prompt, string model, CancellationToken cancellationToken = default)
        {
            try
            {
                var apiKey = await _settingsService.GetApiKeyAsync();
                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new InvalidOperationException("API key not configured");
                }

                var request = CreateRequest(prompt, model);
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, ApiConstants.ContentTypeHeader);

                var url = $"{ApiConstants.GeminiBaseUrl}/models/{model}:{ApiConstants.GeminiGenerateContentEndpoint}?key={apiKey}";
                
                _logger.LogInformation("Sending request to Gemini API with model {Model}", model);
                
                var response = await _httpClient.PostAsync(url, content, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API request failed with status {StatusCode}: {Response}", 
                        response.StatusCode, responseContent);
                    throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
                }

                var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseContent);
                if (geminiResponse == null)
                {
                    throw new JsonException("Failed to deserialize API response");
                }

                _logger.LogInformation("Successfully received response from Gemini API");
                return geminiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Gemini API with model {Model}", model);
                throw;
            }
        }

        public bool IsValidApiKey(string apiKey)
        {
            return !string.IsNullOrWhiteSpace(apiKey) && apiKey.Length > 10;
        }

        public string[] GetAvailableModels()
        {
            return AppConstants.AvailableModels;
        }

        private GeminiRequest CreateRequest(string prompt, string model)
        {
            return new GeminiRequest
            {
                Contents = new[]
                {
                    new Content
                    {
                        Parts = new[]
                        {
                            new Part { Text = prompt }
                        }
                    }
                },
                GenerationConfig = new GenerationConfig
                {
                    Temperature = 0.7,
                    TopK = 40,
                    TopP = 0.95,
                    MaxOutputTokens = 2048
                }
            };
        }

        private string ExtractTextFromResponse(GeminiResponse response)
        {
            if (response?.Candidates == null || response.Candidates.Length == 0)
            {
                return "No response generated";
            }

            var firstCandidate = response.Candidates[0];
            if (firstCandidate?.Content?.Parts == null || firstCandidate.Content.Parts.Length == 0)
            {
                return "Response received but no content found";
            }

            var firstPart = firstCandidate.Content.Parts[0];
            return firstPart?.Text ?? "No text content found";
        }
    }
}
