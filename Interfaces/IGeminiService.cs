using JulyAgent.Models;

namespace JulyAgent.Interfaces
{
    public interface IGeminiService
    {
        Task<string> GenerateContentAsync(string prompt, string model, CancellationToken cancellationToken = default);
        Task<GeminiResponse> GenerateContentDetailedAsync(string prompt, string model, CancellationToken cancellationToken = default);
        bool IsValidApiKey(string apiKey);
        string[] GetAvailableModels();
    }
}
