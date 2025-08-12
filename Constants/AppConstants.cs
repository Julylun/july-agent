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

        // Sample Prompts
        public static readonly string[] SamplePrompts = {
            "You are a helpful AI assistant. Please provide clear, accurate, and helpful responses to the user's questions and requests.",
            "You are a coding expert. Provide detailed, well-structured code examples and explanations. Use best practices and include comments where helpful.",
            "You are a writing assistant. Help users improve their writing with suggestions for clarity, grammar, style, and structure.",
            "You are a language tutor. Help users learn and practice languages with explanations, examples, and corrections.",
            "You are a creative writing coach. Inspire and guide users in developing stories, characters, and creative content.",
            "You are a technical consultant. Provide clear, practical advice on technical problems and solutions."
        };
    }
}
