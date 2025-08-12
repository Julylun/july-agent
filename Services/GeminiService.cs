using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using JulyAgent.Constants;
using JulyAgent.Interfaces;
using JulyAgent.Models;
using System.Linq;
using System.Collections.Generic;

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

                var systemPrompt = await _settingsService.GetPromptAsync();
                var fullPrompt = $"{systemPrompt}\n\nUser request: {prompt}";

                var request = CreateRequest(fullPrompt, model);
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
                // Try to hint safety issues if any
                var safety = response?.PromptFeedback?.SafetyRatings;
                if (safety != null && safety.Length > 0)
                {
                    var categories = string.Join(", ", safety.Select(s => $"{s.Category}:{s.Probability}"));
                    return $"No candidates returned. Safety feedback: {categories}";
                }
                return "No response generated";
            }

            var allTexts = new List<string>();
            foreach (var candidate in response.Candidates)
            {
                var parts = candidate?.Content?.Parts;
                if (parts != null && parts.Length > 0)
                {
                    foreach (var part in parts)
                    {
                        if (!string.IsNullOrWhiteSpace(part?.Text))
                        {
                            allTexts.Add(part!.Text!);
                        }
                    }
                }
            }

            if (allTexts.Count > 0)
            {
                return string.Join("\n\n", allTexts);
            }

            // No text parts found; include finish reason and safety info to help diagnose
            var first = response.Candidates[0];
            var finish = string.IsNullOrWhiteSpace(first?.FinishReason) ? "UNKNOWN" : first!.FinishReason!;
            var candidateSafety = first?.SafetyRatings != null && first.SafetyRatings.Length > 0
                ? string.Join(", ", first.SafetyRatings.Select(s => $"{s.Category}:{s.Probability}"))
                : "none";
            var promptSafety = response.PromptFeedback?.SafetyRatings != null && response.PromptFeedback.SafetyRatings.Length > 0
                ? string.Join(", ", response.PromptFeedback.SafetyRatings.Select(s => $"{s.Category}:{s.Probability}"))
                : "none";

            return $"Response received but no content found (finishReason={finish}, candidateSafety=[{candidateSafety}], promptSafety=[{promptSafety}]).";
        }
    }
}
