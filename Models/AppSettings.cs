using System.Text.Json.Serialization;

namespace JulyAgent.Models
{
    public class AppSettings
    {
        [JsonPropertyName("apiKey")]
        public string? ApiKey { get; set; }

        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("theme")]
        public string Theme { get; set; } = "dark";

        [JsonPropertyName("hotkey")]
        public string Hotkey { get; set; } = "Ctrl+Win+J";

        [JsonPropertyName("autoStart")]
        public bool AutoStart { get; set; } = false;

        [JsonPropertyName("prompt")]
        public string Prompt { get; set; } = "You are a helpful AI assistant. Please provide clear, accurate, and helpful responses to the user's questions and requests.";
    }
}
