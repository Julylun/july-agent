using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using JulyAgent.Forms;
using JulyAgent.Interfaces;
using JulyAgent.Services;

namespace JulyAgent
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            
            var host = CreateHostBuilder().Build();
            var mainForm = host.Services.GetRequiredService<MainForm>();
            
            Application.Run(mainForm);
        }

        private static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Configure logging
                    services.AddLogging(builder =>
                    {
                        builder.AddConsole();
                        builder.AddDebug();
                        builder.SetMinimumLevel(LogLevel.Information);
                    });

                    // Register services
                    services.AddHttpClient();
                    services.AddSingleton<ISettingsService, SettingsService>();
                    services.AddSingleton<IHotkeyService, HotkeyService>();
                    services.AddSingleton<INotifyIconService, NotifyIconService>();
                    services.AddTransient<IGeminiService, GeminiService>();

                    // Register forms
                    services.AddTransient<MainForm>();
                    services.AddTransient<TextInputPopup>();
                    services.AddTransient<ProcessingForm>();
                    services.AddTransient<ResultForm>();
                    services.AddTransient<SettingsForm>();
                });
        }
    }
}
