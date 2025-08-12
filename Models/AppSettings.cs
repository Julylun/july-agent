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
    }
}
