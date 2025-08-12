# July Agent

A Windows desktop application that integrates with Google Gemini AI to provide intelligent text processing capabilities through global hotkeys.

## Features

- **Global Hotkey Support**: Press `Ctrl+Win+J` (configurable) to activate the application from anywhere
- **Gemini AI Integration**: Process text using Google's Gemini AI models
- **System Tray Integration**: Runs in the background with system tray access
- **Theme Support**: Dark and light themes with consistent UI styling
- **Configurable Settings**: API key, model selection, and hotkey customization
- **Modern Architecture**: Built with .NET 8, dependency injection, and clean architecture principles

## Architecture

The application follows clean architecture principles with clear separation of concerns:

```
JulyAgent/
├── Constants/           # Application constants and configurations
├── Forms/              # Windows Forms UI components
├── Interfaces/          # Service contracts and abstractions
├── Models/              # Data models and DTOs
├── Services/            # Business logic and external service integrations
└── Utils/              # Utility classes and helpers
```

### Key Components

- **Models**: Data structures for API requests/responses and application settings
- **Services**: Business logic layer including Gemini API integration and settings management
- **Forms**: UI components with consistent theming and user experience
- **Interfaces**: Contracts for dependency injection and testability
- **Constants**: Centralized configuration and application constants

## Dependencies

- **.NET 8.0**: Modern .NET framework with Windows Forms support
- **Microsoft.Extensions.DependencyInjection**: Dependency injection container
- **Microsoft.Extensions.Hosting**: Application hosting and configuration
- **Microsoft.Extensions.Logging**: Structured logging framework

## Getting Started

### Prerequisites

- .NET 8.0 SDK or Runtime
- Windows 10/11
- Google Gemini API Key

### Installation

1. Clone the repository
2. Build the solution: `dotnet build`
3. Run the application: `dotnet run`

### Configuration

1. Launch the application (it will minimize to system tray)
2. Right-click the system tray icon and select "Settings"
3. Enter your Google Gemini API Key
4. Select your preferred Gemini model
5. Configure the global hotkey if desired

### Usage

1. Press the configured global hotkey (`Ctrl+Win+J` by default)
2. Enter your text in the popup dialog
3. Press Enter or click OK to process with Gemini AI
4. View the AI response in the results window
5. Copy results to clipboard or close the window

## Development

### Project Structure

- **Clean Architecture**: Separation of concerns with interfaces and implementations
- **Dependency Injection**: Service registration and lifetime management
- **Async/Await**: Modern asynchronous programming patterns
- **Error Handling**: Comprehensive exception handling and logging
- **Theme Management**: Centralized UI theming system

### Building

```bash
dotnet build
dotnet run
```

### Testing

The application is designed with testability in mind:
- Interface-based design for mocking
- Dependency injection for service replacement
- Separated business logic from UI concerns

## Contributing

1. Follow the existing code structure and patterns
2. Use dependency injection for new services
3. Implement interfaces for testability
4. Follow .NET coding conventions
5. Add appropriate logging and error handling

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Google Gemini AI for providing the AI capabilities
- .NET community for the excellent framework and tools
- Windows Forms for the desktop application framework
