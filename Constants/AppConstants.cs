namespace JulyAgent.Constants
{
    public static class AppConstants
    {
        public const string AppName = "July Agent";
        public const string AppVersion = "1.0.0";
        public const string CompanyName = "JulyAgent";
        
        // Default settings
        public const string DefaultModel = "gemini-2.5-flash";
        public const string DefaultTheme = "dark";
        public const string DefaultHotkey = "Ctrl+Win+J";
        
        // File paths
        public const string SettingsFileName = "settings.json";
        public const string LogFileName = "julyagent.log";
        
        // UI Constants
        public const int DefaultFormWidth = 800;
        public const int DefaultFormHeight = 600;
        public const int DefaultButtonHeight = 35;
        public const int DefaultButtonWidth = 80;
        
        // Colors
        public static class Colors
        {
            public const int DarkBackground = 45;
            public const int DarkForeground = 30;
            public const int PrimaryBlue = 0x007ACC;
            public const int SecondaryGray = 60;
            public const int SuccessGreen = 0x009600;
            public const int WarningOrange = 0xFF8C00;
            public const int ErrorRed = 0xE51400;
        }
        
        // Gemini Models
        public static readonly string[] AvailableModels = {
            "gemini-2.5-flash",
            "gemini-2.5-pro",
            "gemini-1.5-flash",
            "gemini-1.5-pro",
            "gemini-1.0-pro",
            "gemini-1.0-pro-vision"
        };
    }
}
