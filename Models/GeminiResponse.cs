using System.Text.Json.Serialization;

namespace JulyAgent.Models
{
    public class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public Candidate[]? Candidates { get; set; }

        [JsonPropertyName("promptFeedback")]
        public PromptFeedback? PromptFeedback { get; set; }

        [JsonPropertyName("usageMetadata")]
        public UsageMetadata? UsageMetadata { get; set; }
    }

    public class Candidate
    {
        [JsonPropertyName("content")]
        public ResponseContent? Content { get; set; }

        [JsonPropertyName("finishReason")]
        public string? FinishReason { get; set; }

        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("safetyRatings")]
        public SafetyRating[]? SafetyRatings { get; set; }
    }

    public class ResponseContent
    {
        [JsonPropertyName("parts")]
        public ResponsePart[]? Parts { get; set; }

        [JsonPropertyName("role")]
        public string? Role { get; set; }
    }

    public class ResponsePart
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }

    public class PromptFeedback
    {
        [JsonPropertyName("safetyRatings")]
        public SafetyRating[]? SafetyRatings { get; set; }
    }

    public class SafetyRating
    {
        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("probability")]
        public string? Probability { get; set; }
    }

    public class UsageMetadata
    {
        [JsonPropertyName("promptTokenCount")]
        public int PromptTokenCount { get; set; }

        [JsonPropertyName("candidatesTokenCount")]
        public int CandidatesTokenCount { get; set; }

        [JsonPropertyName("totalTokenCount")]
        public int TotalTokenCount { get; set; }
    }
}
