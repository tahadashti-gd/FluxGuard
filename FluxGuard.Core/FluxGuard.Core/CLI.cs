using Figgle;
using Microsoft.VisualBasic.Logging;
using Spectre.Console;
using System.Diagnostics;

namespace FluxGuard.Core
{
    public class CLI
    {
        public static void StartCLI()
        {
            string logo = FiggleFonts.Standard.Render("Flux Guard");
            AnsiConsole.MarkupLine($"[bold green]{logo}[/]");
            AnsiConsole.MarkupLine("[bold green]Welcome to FluxGuard Bot![/]");
            AnsiConsole.MarkupLine("[dim]Your security and control assistant.[/]");
            AnsiConsole.MarkupLine("[bold cyan]Type [green]help[/] for available commands.[/]");

            while (true)
            {
                string command = AnsiConsole.Prompt(
                    new TextPrompt<string>("> [bold yellow]Enter command[/]:")
                        .AllowEmpty());
                var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) return;

                switch (parts[0].ToLower())
                {
                    case "start-bot":
                        MainCore botCore = new MainCore();
                        botCore.StartBot();
                        Process.Start("FluxGuard-GUI", "FluxGuard-GUI.exe");
                        //LoggerService.log("FluxGuard started at {Time}", DateTime.UtcNow);

                        AnsiConsole.MarkupLine("[green]Bot is running...[/]");
                        break;
                    case "stop-bot":
                        AnsiConsole.MarkupLine("[green]Bot is stoping...[/]");
                        break;
                    case "status":
                        AnsiConsole.MarkupLine("[green]Bot is running...[/]");
                        break;
                    case "logs":
                        if (parts.Length > 1 && int.TryParse(parts[1], out int hours))
                        {
                            Logger.ShowRecentLogs(hours);
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]❌ Invalid number of hours![/]");
                        }
                        break;
                    case "config":
                        DataManager.ShowConfig();
                        var configChoice = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("[bold blue]Select a field to change[/]")
                                .AddChoices($"Telegram Bot Token", $"User ChatID"));

                        string newValue = AnsiConsole.Ask<string>("[bold blue]Enter new value[/]:");
                        switch (configChoice.ToLower())
                        {
                            case "telegram bot token":
                                MainCore.BotToken = newValue;
                                DataManager.SetConfigValue("telegram_bot_token", newValue);
                                AnsiConsole.MarkupLine($"[green]Telegram Bot Token has been updated to:[/] [bold]{MainCore.BotToken}[/]");
                                break;
                            case "user chatid":
                                MainCore.userChatID = newValue;
                                DataManager.SetConfigValue("user_chat_id", newValue);
                                AnsiConsole.MarkupLine($"[green]User Chat ID has been updated to:[/] [bold]{MainCore.userChatID}[/]");
                                break;
                            default:
                                AnsiConsole.MarkupLine("[red]Unknown option![/]");
                                break;
                        }
                        break;
                    //case "setting":
                    //    DataManager.ShowConfig();
                    //    var choice = AnsiConsole.Prompt(
                    //        new SelectionPrompt<string>()
                    //            .Title("[bold blue]Select a field to change[/]")
                    //            .AddChoices($"Telegram Bot Token", $"User ChatID"));

                    //    string newValue = AnsiConsole.Ask<string>("[bold blue]Enter new value[/]:");

                    //    switch (choice.ToLower())
                    //    {
                    //        case "telegram bot token":
                    //            MainCore.BotToken = newValue;
                    //            DataManager.SetConfigValue("telegram_bot_token", newValue);
                    //            AnsiConsole.MarkupLine($"[green]Telegram Bot Token has been updated to:[/] [bold]{MainCore.BotToken}[/]");
                    //            break;
                    //        case "user chatid":
                    //            MainCore.userChatID = newValue;
                    //            DataManager.SetConfigValue("user_chat_id", newValue);
                    //            AnsiConsole.MarkupLine($"[green]User Chat ID has been updated to:[/] [bold]{MainCore.userChatID}[/]");
                    //            break;
                    //        default:
                    //            AnsiConsole.MarkupLine("[red]Unknown option![/]");
                    //            break;
                    //    }
                    //    break;
                    case "exit":
                        AnsiConsole.MarkupLine("[red]Exiting...[/]");
                        return;
                    default:
                        AnsiConsole.MarkupLine("[red]Unknown command![/]");
                        break;
                }
            }
        }
    }
}
