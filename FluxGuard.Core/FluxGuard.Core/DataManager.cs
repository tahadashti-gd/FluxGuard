using Spectre.Console;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluxGuard.GUI;

namespace FluxGuard.Core
{
    public class DataManager
    {
        // File paths for config and settings
        private static string configFilePath = "config.json";
        private static string settingFilePath = "setting.json";

        // Public properties to access config and settings
        public static ConfigModel Config { get; private set; }
        public static SettingModel Setting { get; private set; }

        // Model for application settings
        public class SettingModel
        {
            [JsonPropertyName("telegram_bot_language")]
            public string? TelegramBotLanguage { get; set; } = "en";

            [JsonPropertyName("automatic_start")]
            public bool AutomaticStart { get; set; } = false;
        }

        // Model for configuration
        public class ConfigModel
        {
            [JsonPropertyName("telegram_bot_token")]
            public string? TelegramBotToken { get; set; } = "";

            [JsonPropertyName("user_chat_id")]
            public string? UserChatId { get; set; } = "";
        }

        // Ensure config file exists, otherwise create one with default values
        private static void EnsureConfigFile()
        {
            if (!File.Exists(configFilePath))
            {
                Config = new ConfigModel();
                string jsonString = JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(configFilePath, jsonString);
                LoggerService.LogInformation("Config file created with default values.");
            }
        }

        // Ensure setting file exists, otherwise create one with default values
        private static void EnsureSettingFile()
        {
            if (!File.Exists(settingFilePath))
            {
                Setting = new SettingModel();
                string jsonString = JsonSerializer.Serialize(Setting, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(settingFilePath, jsonString);
                LoggerService.LogInformation("Setting file created with default values.");
            }
        }

        // Load config file
        public static void LoadConfig()
        {
            EnsureConfigFile();
            try
            {
                string jsonContent = File.ReadAllText(configFilePath);
                Config = JsonSerializer.Deserialize<ConfigModel>(jsonContent);
                Form1.userChatId = Config.UserChatId;
                Form1.telegramBotToken = Config.TelegramBotToken;
                LoggerService.LogInformation("Config loaded.");
            }
            catch (Exception ex)
            {
                Config = null;
                LoggerService.LogError(ex, "Error loading config");
            }
        }

        // Load setting file
        public static void LoadSetting()
        {
            EnsureSettingFile();
            try
            {
                string jsonContent = File.ReadAllText(settingFilePath);
                Setting = JsonSerializer.Deserialize<SettingModel>(jsonContent);
                LoggerService.LogInformation("Settings loaded.");
            }
            catch (Exception ex)
            {
                LoggerService.LogError(ex, "Error loading settings");
            }
        }

        // Initialize configuration and settings
        public static void Initialize()
        {
            LoadConfig();
            LoadSetting();
        }

        // Display current configuration
        public static void ShowConfig()
        {
            AnsiConsole.MarkupLine("[bold cyan]Current Configuration:[/]");
            AnsiConsole.MarkupLine($"[green]Telegram Bot Token:[/] {Config.TelegramBotToken}");
            AnsiConsole.MarkupLine($"[green]User Chat ID:[/] {Config.UserChatId}");
        }

        // Display current settings
        public static void ShowSetting()
        {
            AnsiConsole.MarkupLine("[bold cyan]Current Settings:[/]");
            AnsiConsole.MarkupLine($"[green]Telegram Bot Language:[/] {Setting.TelegramBotLanguage}");
            AnsiConsole.MarkupLine($"[green]Automatic Start:[/] {Setting.AutomaticStart}");

        }

        // Update a config value
        public static void SetConfigValue(string key, dynamic value)
        {
            try
            {
                Dictionary<string, dynamic> configData;

                // Read existing config file
                if (File.Exists(configFilePath))
                {
                    string existingJson = File.ReadAllText(configFilePath);
                    configData = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(existingJson) ?? new Dictionary<string, dynamic>();
                }
                else
                {
                    configData = new Dictionary<string, dynamic>();
                }

                // Check if the value is the same as the existing one
                if (configData.ContainsKey(key) && ConvertJsonElement(configData[key]) == value)
                {
                    LoggerService.LogInformation($"No change detected for '{key}', skipping update.");
                    return;
                }

                // Update key value
                configData[key] = value;

                // Save updated config file
                string jsonString = JsonSerializer.Serialize(configData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(configFilePath, jsonString);
                LoggerService.LogSecurity($"Set {key} to {value} in config.");
                LoadConfig();
            }
            catch (Exception ex)
            {
                LoggerService.LogError(ex, "Error in set data config");
            }
        }

        // Update a setting value
        public static void SetSettingValue(string key, dynamic value)
        {
            try
            {
                Dictionary<string, dynamic> settingData;

                // Read existing setting file
                if (File.Exists(settingFilePath))
                {
                    string existingJson = File.ReadAllText(settingFilePath);
                    settingData = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(existingJson) ?? new Dictionary<string, dynamic>();
                }
                else
                {
                    settingData = new Dictionary<string, dynamic>();
                }

                // Check if the value is the same as the existing one
                if (settingData.ContainsKey(key) && ConvertJsonElement(settingData[key]) == value)
                {
                    LoggerService.LogInformation($"No change detected for '{key}', skipping update.");
                    return;
                }

                // Update key value
                settingData[key] = value;

                // Save updated setting file
                string jsonString = JsonSerializer.Serialize(settingData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(settingFilePath, jsonString);
                LoggerService.LogSecurity($"Set {key} to {value} in setting.");
                Initialize();
            }
            catch (Exception ex)
            {
                LoggerService.LogError(ex, "Error in set data setting");
            }
        }

        // Convert JsonElement to appropriate type
        private static dynamic ConvertJsonElement(dynamic element)
        {
            if (element is JsonElement jsonElement)
            {
                return jsonElement.ValueKind switch
                {
                    JsonValueKind.String => jsonElement.GetString(),
                    JsonValueKind.Number => jsonElement.GetDouble(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    _ => element // Return as is for unsupported types
                };
            }
            return element;
        }
    }
}
