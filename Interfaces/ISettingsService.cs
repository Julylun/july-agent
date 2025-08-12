using JulyAgent.Models;

namespace JulyAgent.Interfaces
{
    public interface ISettingsService
    {
        Task<AppSettings> LoadSettingsAsync();
        Task SaveSettingsAsync(AppSettings settings);
        Task<string?> GetApiKeyAsync();
        Task<string> GetModelAsync();
        Task<string> GetThemeAsync();
        Task<string> GetHotkeyAsync();
        Task<bool> GetAutoStartAsync();
        Task UpdateApiKeyAsync(string apiKey);
        Task UpdateModelAsync(string model);
        Task UpdateThemeAsync(string theme);
        Task UpdateHotkeyAsync(string hotkey);
        Task UpdateAutoStartAsync(bool autoStart);
        string GetSettingsFilePath();
        bool SettingsFileExists();
    }
}
