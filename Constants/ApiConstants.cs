namespace JulyAgent.Constants
{
    public static class ApiConstants
    {
        public const string GeminiBaseUrl = "https://generativelanguage.googleapis.com/v1beta";
        public const string GeminiGenerateContentEndpoint = "generateContent";
        
        // API Headers
        public const string ContentTypeHeader = "application/json";
        public const string UserAgentHeader = "JulyAgent/1.0.0";
        
        // Request timeouts
        public const int DefaultTimeoutSeconds = 30;
        public const int LongTimeoutSeconds = 60;
        
        // Rate limiting
        public const int MaxRequestsPerMinute = 60;
        public const int MaxTokensPerRequest = 8192;
        
        // Error messages
        public const string ApiKeyRequiredMessage = "Please configure your Gemini API Key in Settings first!";
        public const string ApiErrorPrefix = "API Error: ";
        public const string NetworkErrorPrefix = "Network Error: ";
        public const string ParsingErrorPrefix = "Parsing Error: ";
    }
}
