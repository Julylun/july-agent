using System.Text.Json.Serialization;

namespace JulyAgent.Models
{
    public class GeminiRequest
    {
        [JsonPropertyName("contents")]
        public Content[] Contents { get; set; } = Array.Empty<Content>();

        [JsonPropertyName("generationConfig")]
        public GenerationConfig? GenerationConfig { get; set; }

        [JsonPropertyName("safetySettings")]
        public SafetySetting[]? SafetySettings { get; set; }
    }

    public class Content
    {
        [JsonPropertyName("parts")]
        public Part[] Parts { get; set; } = Array.Empty<Part>();

        [JsonPropertyName("role")]
        public string? Role { get; set; }
    }

    public class Part
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }

    public class GenerationConfig
    {
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; } = 0.7;

        [JsonPropertyName("topK")]
        public int TopK { get; set; } = 40;

        [JsonPropertyName("topP")]
        public double TopP { get; set; } = 0.95;

        [JsonPropertyName("maxOutputTokens")]
        public int MaxOutputTokens { get; set; } = 2048;
    }

    public class SafetySetting
    {
        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("threshold")]
        public string Threshold { get; set; } = "BLOCK_MEDIUM_AND_ABOVE";
    }
}
