using System.Text.Json;
using FluxGuard.Core.Models;
using FluxGuard.Core.Utilities;
using Spectre.Console;

namespace FluxGuard.Core.Services
{
    public class ConfigService
    {
        private static void EnsureFile()
        {
            if (!File.Exists(ConfigModel.FilePath))
            {
                ConfigModel.Config = new ConfigModel.Model();
                string jsonString = JsonSerializer.Serialize(ConfigModel.Config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ConfigModel.FilePath, jsonString);
                LogService.LogInformation("Config file created with default values.");
            }
        }

        public static void Load()
        {
            EnsureFile();
            try
            {
                string jsonContent = File.ReadAllText(ConfigModel.FilePath);
                ConfigModel.Config = JsonSerializer.Deserialize<ConfigModel.Model>(jsonContent);
                LogService.LogInformation("Config loaded.");
            }
            catch (Exception ex)
            {
                ConfigModel.Config = null;
                LogService.LogError(ex, "Error loading config");
            }
        }

        public static void Show()
        {
            AnsiConsole.MarkupLine("[bold cyan]Current Configuration:[/]");
            AnsiConsole.MarkupLine($"[green]Telegram Bot Token:[/] {ConfigModel.Config.TelegramBotToken}");
            AnsiConsole.MarkupLine($"[green]User Chat ID:[/] {ConfigModel.Config.UserChatId}");
        }

        public static void SetValue(string key, dynamic value)
        {
            try
            {
                Dictionary<string, dynamic> configData;

                if (File.Exists(ConfigModel.FilePath))
                {
                    string existingJson = File.ReadAllText(ConfigModel.FilePath);
                    configData = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(existingJson) ?? new Dictionary<string, dynamic>();
                }
                else
                {
                    configData = new Dictionary<string, dynamic>();
                }

                if (configData.ContainsKey(key) && ConvertJsonElement.Convert(configData[key]) == value)
                {
                    LogService.LogInformation($"No change detected for '{key}', skipping update.");
                    return;
                }

                configData[key] = value;
                string jsonString = JsonSerializer.Serialize(configData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ConfigModel.FilePath, jsonString);
                LogService.LogSecurity($"Set {key} to {value} in config.");
                Load();
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, "Error in set data config");
            }
        }
    }
}