using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Cli;
using FluxGuard.Core.Services;
using FluxGuard.Core.Models;
using FluxGuard.Core.Utilities;

namespace FluxGuard.CLI
{
    internal class CliManager
    {
        public static async Task RunAsync(string[] args)
        {
            var app = new CommandApp();
            app.Configure(config =>
            {
                config.SetApplicationName("fluxguard");
                config.AddCommand<StartBotCommand>("start").WithDescription("Starts the FluxGuard bot and GUI.");
                config.AddCommand<StopBotCommand>("stop").WithDescription("Stops the running bot.");
                config.AddCommand<StatusCommand>("status").WithDescription("Displays the current status of the bot.");
                config.AddCommand<LogsCommand>("logs").WithDescription("Displays recent logs from the last hours.");
                config.AddCommand<ConfigCommand>("config").WithDescription("Manage bot configurations.");
                config.AddCommand<SettingsCommand>("settings").WithDescription("Modify bot settings.");
                config.AddCommand<ExitCommand>("exit").WithDescription("Exits the application.");
            });

            // Interactive loop to get user input
            while (true)
            {
                // Display prompt to get command
                string input = AnsiConsole.Prompt(
                    new TextPrompt<string>("> [bold yellow]Enter command[/]:")
                        .AllowEmpty());

                // If the user didn’t enter anything, continue
                if (string.IsNullOrWhiteSpace(input))
                    continue;

                // Convert input to an array of arguments
                var commandArgs = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                try
                {
                    // Execute the command with CommandApp
                    await app.RunAsync(commandArgs);
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
                }
            }
        }

        // Command to start the bot
        private class StartBotCommand : AsyncCommand
        {
            public override async Task<int> ExecuteAsync(CommandContext context)
            {
                TelegramClientService clientService = new TelegramClientService();
                clientService.StartBot();
                AnsiConsole.MarkupLine("[green]Bot is running...[/]");
                return 0;
            }
        }

        // Command to stop the bot
        private class StopBotCommand : AsyncCommand
        {
            public override async Task<int> ExecuteAsync(CommandContext context)
            {
                AnsiConsole.MarkupLine("[red]Bot was stopped...[/]");
                return 0;
            }
        }

        // Command to check bot status
        private class StatusCommand : AsyncCommand
        {
            public override async Task<int> ExecuteAsync(CommandContext context)
            {
                //AnsiConsole.MarkupLine(bot); // Add real logic here
                return 0;
            }
        }

        // Command to display logs
        private class LogsCommand : AsyncCommand<LogsCommand.Settings>
        {
            public class Settings : CommandSettings
            {
                [CommandArgument(0, "<hours>")]
                public int Hours { get; set; }
            }

            public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
            {
                Logger.ShowRecentLogs(settings.Hours);
                return 0;
            }
        }

        // Command to manage settings
        private class SettingsCommand : AsyncCommand
        {
            public override async Task<int> ExecuteAsync(CommandContext context)
            {
                SettingService.Show();
                var settingChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold blue]Select a field to change[/]")
                        .AddChoices("Telegram Bot Language", "Automatic Start", "Back"));

                if (settingChoice == "Back") return 0;

                switch (settingChoice)
                {
                    case "Telegram Bot Language":
                        string lang = AnsiConsole.Ask<string>("[bold blue]Enter new value[/]:");
                        SettingService.SetValue("telegram_bot_language", lang);
                        AnsiConsole.MarkupLine($"[green]Updated to:[/] [bold]{SettingModel.Setting.BotLanguage}[/]");
                        LanguageService.Initialize();
                        break;
                    case "Automatic Start":
                        bool autoStart = AnsiConsole.Prompt(
                            new SelectionPrompt<bool>()
                                .Title("[bold blue]True/False?[/]")
                                .AddChoices(true, false));
                        SettingService.SetValue("automatic_start", autoStart);
                        AnsiConsole.MarkupLine($"[green]Updated to:[/] [bold]{SettingModel.Setting.AutomaticStart}[/]");
                        break;
                }
                return 0;
            }
        }

        // Command to manage configuration
        private class ConfigCommand : AsyncCommand
        {
            public override async Task<int> ExecuteAsync(CommandContext context)
            {
                ConfigService.Show();
                var configChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold blue]Select a field to change[/]")
                        .AddChoices("Telegram Bot Token", "User ChatID", "Back"));

                if (configChoice == "Back") return 0;

                switch (configChoice)
                {
                    case "Telegram Bot Token":
                        string token = AnsiConsole.Ask<string>("[bold blue]Enter new Bot Token[/]:");
                        ConfigService.SetValue("telegram_bot_token", token);
                        AnsiConsole.MarkupLine($"[green]Updated to:[/] [bold]{ConfigModel.Config.TelegramBotToken}[/]");
                        break;
                    case "User ChatID":
                        long chatId = AnsiConsole.Ask<long>("[bold blue]Enter new User ChatID[/]:");
                        ConfigService.SetValue("user_chat_id", chatId);
                        AnsiConsole.MarkupLine($"[green]Updated to:[/] [bold]{ConfigModel.Config.UserChatId}[/]");
                        break;
                }
                return 0;
            }
        }

        // Command to exit the application
        private class ExitCommand : AsyncCommand
        {
            public override async Task<int> ExecuteAsync(CommandContext context)
            {
                AnsiConsole.MarkupLine("[red]Exiting...[/]");
                Environment.Exit(0);
                return 0;
            }
        }
    }
}