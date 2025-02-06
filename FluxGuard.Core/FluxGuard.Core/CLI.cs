using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluxGuard.Core
{
    public class CLI
    {
        public static void StartCLI()
        {
            AnsiConsole.MarkupLine("[bold green]FluxGuard CLI[/]");

            while (true)
            {
                string command = AnsiConsole.Prompt(
                    new TextPrompt<string>("> [bold yellow]Enter command[/]:")
                        .AllowEmpty());
                var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) return;


                switch (parts[0].ToLower())
                {
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
                        string key = AnsiConsole.Ask<string>("[bold blue]Enter config key:[/]");
                        string value = AnsiConsole.Ask<string>("[bold blue]Enter value:[/]");
                        AnsiConsole.MarkupLine($"🔹 [bold]{key}[/] set to [yellow]{value}[/]");
                        break;

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
