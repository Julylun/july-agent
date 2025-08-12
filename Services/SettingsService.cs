using System.Text.Json;
using Microsoft.Extensions.Logging;
using JulyAgent.Constants;
using JulyAgent.Interfaces;
using JulyAgent.Models;

namespace JulyAgent.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly ILogger<SettingsService> _logger;
        private readonly string _settingsFilePath;

        public SettingsService(ILogger<SettingsService> logger)
        {
            _logger = logger;
            _settingsFilePath = GetSettingsFilePath();
        }

        public async Task<AppSettings> LoadSettingsAsync()
        {
            try
            {
                if (!SettingsFileExists())
                {
                    _logger.LogInformation("Settings file not found, creating default settings");
                    return CreateDefaultSettings();
                }

                var json = await File.ReadAllTextAsync(_settingsFilePath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json);
                
                if (settings == null)
                {
                    _logger.LogWarning("Failed to deserialize settings, using defaults");
                    return CreateDefaultSettings();
                }

                _logger.LogInformation("Settings loaded successfully");
                return settings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading settings, using defaults");
                return CreateDefaultSettings();
            }
        }

        public async Task SaveSettingsAsync(AppSettings settings)
        {
            try
            {
                var settingsDir = Path.GetDirectoryName(_settingsFilePath);
                if (!string.IsNullOrEmpty(settingsDir) && !Directory.Exists(settingsDir))
                {
                    Directory.CreateDirectory(settingsDir);
                }

                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_settingsFilePath, json);
                
                _logger.LogInformation("Settings saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving settings");
                throw;
            }
        }

        public async Task<string?> GetApiKeyAsync()
        {
            var settings = await LoadSettingsAsync();
            return settings.ApiKey;
        }

        public async Task<string> GetModelAsync()
        {
            var settings = await LoadSettingsAsync();
            return settings.Model ?? AppConstants.DefaultModel;
        }

        public async Task<string> GetThemeAsync()
        {
            var settings = await LoadSettingsAsync();
            return settings.Theme ?? AppConstants.DefaultTheme;
        }

        public async Task<string> GetHotkeyAsync()
        {
            var settings = await LoadSettingsAsync();
            return settings.Hotkey ?? AppConstants.DefaultHotkey;
        }

        public async Task<bool> GetAutoStartAsync()
        {
            var settings = await LoadSettingsAsync();
            return settings.AutoStart;
        }

        public async Task<string> GetPromptAsync()
        {
            var settings = await LoadSettingsAsync();
            return settings.Prompt ?? "You are a helpful AI assistant. Please provide clear, accurate, and helpful responses to the user's questions and requests.";
        }

        public async Task UpdateApiKeyAsync(string apiKey)
        {
            var settings = await LoadSettingsAsync();
            settings.ApiKey = apiKey;
            await SaveSettingsAsync(settings);
        }

        public async Task UpdateModelAsync(string model)
        {
            var settings = await LoadSettingsAsync();
            settings.Model = model;
            await SaveSettingsAsync(settings);
        }

        public async Task UpdateThemeAsync(string theme)
        {
            var settings = await LoadSettingsAsync();
            settings.Theme = theme;
            await SaveSettingsAsync(settings);
        }

        public async Task UpdateHotkeyAsync(string hotkey)
        {
            var settings = await LoadSettingsAsync();
            settings.Hotkey = hotkey;
            await SaveSettingsAsync(settings);
        }

        public async Task UpdateAutoStartAsync(bool autoStart)
        {
            var settings = await LoadSettingsAsync();
            settings.AutoStart = autoStart;
            await SaveSettingsAsync(settings);
        }

        public async Task UpdatePromptAsync(string prompt)
        {
            var settings = await LoadSettingsAsync();
            settings.Prompt = prompt;
            await SaveSettingsAsync(settings);
        }

        public string GetSettingsFilePath()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appDataPath, AppConstants.CompanyName, AppConstants.SettingsFileName);
        }

        public bool SettingsFileExists()
        {
            return File.Exists(_settingsFilePath);
        }

        private AppSettings CreateDefaultSettings()
        {
            return new AppSettings
            {
                ApiKey = string.Empty,
                Model = AppConstants.DefaultModel,
                Theme = AppConstants.DefaultTheme,
                Hotkey = AppConstants.DefaultHotkey,
                AutoStart = false
            };
        }
    }
}
