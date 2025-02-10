using System.Diagnostics;
using Figgle;
using Spectre.Console;

namespace FluxGuard.Core
{
    public class CLI
    {
        // Starts the CLI interface
        public static void StartCLI()
        {
            string logo = FiggleFonts.Standard.Render("Flux Guard");
            AnsiConsole.MarkupLine($"[bold green]{logo}[/]");
            AnsiConsole.MarkupLine("[bold green]Welcome to FluxGuard Bot![/]");
            AnsiConsole.MarkupLine("[dim]Your security and control assistant.[/]");
            AnsiConsole.MarkupLine("[bold cyan]Type [green]help[/] for available commands.[/]");

            while (true)
            {
                // Prompt user for command input
                string command = AnsiConsole.Prompt(
                    new TextPrompt<string>("> [bold yellow]Enter command[/]:")
                        .AllowEmpty());
                var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) return;

                // Handle user commands
                switch (parts[0].ToLower())
                {
                    case "bot-start":
                        StartBot();
                        break;
                    case "bot-stop":
                        StopBot();
                        break;
                    case "status":
                        BotStatus();
                        break;
                    case "logs":
                        GetRecentLogs(parts);
                        break;
                    case "config":
                        ManageConfig();
                        break;
                    case "settings":
                        ManageSettings();
                        break;
                    case "help":
                        ShowHelp();
                        break;
                    case "exit":
                        ExitApp();
                        return;
                    default:
                        AnsiConsole.MarkupLine("[red]Unknown command![/]");
                        break;
                }
            }
        }

        // Handles application exit
        private static void ExitApp()
        {
            AnsiConsole.MarkupLine("[red]Exiting...[/]");
        }

        // Manages bot settings
        private static void ManageSettings()
        {
            DataManager.ShowSetting();
            var settingChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("[bold blue]Select a field to change[/]")
                .AddChoices($"Telegram Bot Language", "Automatic start", "Back"));
            if (settingChoice.ToLower() != "back")
            {
                switch (settingChoice.ToLower())
                {
                    case "telegram bot language":
                        string settingValue = AnsiConsole.Ask<string>("[bold blue]Enter new value[/]:");
                        MainCore.BotLanguages = settingValue;
                        DataManager.SetSettingValue("telegram_bot_language", settingValue);
                        AnsiConsole.MarkupLine($"[green]Telegram Bot language has been updated to:[/] [bold]{DataManager.Setting.TelegramBotLanguage}[/]");
                        break;
                    case "automatic start":
                        var autoStart = AnsiConsole.Prompt(
                            new SelectionPrompt<bool>()
                            .Title("[bold blue]True/False ?[/]")
                            .AddChoices(true, false));
                        DataManager.SetSettingValue("automatic_start", autoStart);
                        AnsiConsole.MarkupLine($"[green]Automatic start has been updated to:[/] [bold]{DataManager.Setting.AutomaticStart}[/]");
                        break;
                    default:
                        AnsiConsole.MarkupLine("[red]Unknown option![/]");
                        break;
                }
            }
            else
                return;
        }

        // Manages bot configurations
        private static void ManageConfig()
        {
            DataManager.ShowConfig();
            var configChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("[bold blue]Select a field to change[/]")
                .AddChoices($"Telegram Bot Token", $"User ChatID", "Back"));
            switch (configChoice.ToLower())
            {
                case "telegram bot token":
                    string botTokenValue = AnsiConsole.Ask<string>("[bold blue]Enter new Bot Token[/]:");
                    MainCore.BotToken = botTokenValue;
                    DataManager.SetConfigValue("telegram_bot_token", botTokenValue);
                    AnsiConsole.MarkupLine($"[green]Telegram Bot Token has been updated to:[/] [bold]{DataManager.Config.TelegramBotToken}[/]");
                    break;
                case "user chatid":
                    string chatIDValue = AnsiConsole.Ask<string>("[bold blue]Enter new User ChatID[/]:");
                    MainCore.userChatID = long.Parse(chatIDValue);
                    DataManager.SetConfigValue("user_chat_id", chatIDValue);
                    AnsiConsole.MarkupLine($"[green]User Chat ID has been updated to:[/] [bold]{DataManager.Config.UserChatId}[/]");
                    break;
                case "back":
                    break;
                default:
                    AnsiConsole.MarkupLine("[red]Unknown option![/]");
                    break;
            }
        }

        // Displays available commands
        private static void ShowHelp()
        {
            var table = new Table();
            table.AddColumn("[bold cyan]Command[/]");
            table.AddColumn("[bold yellow]Description[/]");

            table.AddRow("bot-start", "Starts the FluxGuard bot and GUI.");
            table.AddRow("bot-stop", "Stops the running bot.");
            table.AddRow("status", "Displays the current status of the bot.");
            table.AddRow("logs hours", "Displays recent logs from the last hours.");
            table.AddRow("config", "Manage bot configurations such as API tokens and Chat IDs.");
            table.AddRow("settings", "Modify bot settings such as language and auto-start.");
            table.AddRow("help", "Shows this help menu.");
            table.AddRow("exit", "Exits the application.");

            AnsiConsole.Write(table);
        }

        // Fetches recent logs
        private static void GetRecentLogs(string[] parts)
        {
            if (parts.Length > 1 && int.TryParse(parts[1], out int hours))
            {
                Logger.ShowRecentLogs(hours);
            }
            else
            {
                AnsiConsole.MarkupLine("[red]❌ Invalid number of hours![/]");
            }
        }

        // Displays bot status
        private static void BotStatus()
        {
            AnsiConsole.MarkupLine("[green]Bot is running...[/]");
        }

        // Stops the bot
        private static void StopBot()
        {
            AnsiConsole.MarkupLine("[red]Bot was stopped...[/]");
        }

        // Starts the bot
        public static void StartBot()
        {
            MainCore botCore = new MainCore();
            botCore.StartBot();
            Process.Start("FluxGuard-GUI", "FluxGuard-GUI.exe");
            LoggerService.LogInformation($"FluxGuard started at {DateTime.UtcNow}");
            AnsiConsole.MarkupLine("[green]Bot is running...[/]");
        }
    }
}