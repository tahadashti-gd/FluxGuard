using System.Text.Json;
using FluxGuard.Core.Models;
using FluxGuard.Core.Utilities;
using Spectre.Console;

namespace FluxGuard.Core.Services
{
    internal class SettingService
    {
        private static void EnsureFile()
        {
            if (!File.Exists(SettingModel.FilePath))
            {
                SettingModel.Setting = new SettingModel.Model();
                string jsonString = JsonSerializer.Serialize(SettingModel.Setting, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingModel.FilePath, jsonString);
                LogService.LogInformation("Setting file created with default values.");
            }
        }

        public static void Load()
        {
            EnsureFile();
            try
            {
                string jsonContent = File.ReadAllText(SettingModel.FilePath);
                SettingModel.Setting = JsonSerializer.Deserialize<SettingModel.Model>(jsonContent);
                LogService.LogInformation("Setting loaded.");
            }
            catch (Exception ex)
            {
                SettingModel.Setting = null;
                LogService.LogError(ex, "Error loading setting");
            }
        }

        public static void Show()
        {
            AnsiConsole.MarkupLine("[bold cyan]Current Settings:[/]");
            AnsiConsole.MarkupLine($"[green]Telegram Bot Language:[/] {SettingModel.Setting.BotLanguage}");
            AnsiConsole.MarkupLine($"[green]Automatic Start:[/] {SettingModel.Setting.AutomaticStart}");
        }

        public static void SetValue(string key, dynamic value)
        {
            try
            {
                Dictionary<string, dynamic> configData;

                if (File.Exists(SettingModel.FilePath))
                {
                    string existingJson = File.ReadAllText(SettingModel.FilePath);
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
                File.WriteAllText(SettingModel.FilePath, jsonString);
                LogService.LogSecurity($"Set {key} to {value} in settings.");
                Load();
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, "Error in set data setting");
            }
        }
    }
}