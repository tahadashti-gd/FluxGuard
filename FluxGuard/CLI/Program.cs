using System.Diagnostics;
using FluxGuard.Core.Utilities;
using FluxGuard.Core.Services;
using FluxGuard.Core.Models;
using FluxGuard.CLI;
using Figgle;
using Spectre.Console;

namespace FluxGuard.CLI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Process.Start(AppContext.BaseDirectory + "/GUI.exe");

                // Initialize the logging system
                Logger.Initialize();

                // Initialize the data management system
                SettingService.Load();
                ConfigService.Load();

                LanguageService.Initialize();

                //Check if the application is set to start automatically
                if (SettingModel.Setting.AutomaticStart)
                {
                    // Start the bot if automatic start is enabled
                    TelegramClientService clientService = new TelegramClientService();
                    clientService.StartBot();
                }

                // Logo
                string logo = FiggleFonts.Standard.Render("Flux Guard");
                AnsiConsole.MarkupLine($"[bold green]{logo}[/]");
                AnsiConsole.MarkupLine("[bold green]Welcome to FluxGuard Bot![/]");
                AnsiConsole.MarkupLine("[dim]Your security and control assistant.[/]");

                // Run CliManager
                await CliManager.RunAsync(args);


            }
            catch (Exception ex)
            {
                // Log the error with details for debugging
                LogService.LogError(ex, "Initializing program");
            }
        }
    }
}
