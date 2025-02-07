using Spectre.Console;
using System.Text.Json;
using System.IO;
using System.Text.Json.Serialization;

namespace FluxGuard.Core
{
    public class DataManager
    {
        private static string configFilePath = "config.json";
        private static string settingFilePath = "setting.json";

        public class ConfigModel
        {
            [JsonPropertyName("telegram_bot_token")]
            public string TelegramBotToken { get; set; }

            [JsonPropertyName("user_chat_id")]
            public string UserChatId { get; set; }
        }
        public class SettingModel
        {
            
        }
        public static ConfigModel LoadConfig()
        {
            try
            {
                string jsonContent = File.ReadAllText(configFilePath);
                return JsonSerializer.Deserialize<ConfigModel>(jsonContent); // استفاده از System.Text.Json
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static void ShowConfig()
        {
            AnsiConsole.MarkupLine("[bold cyan]Current Configuration:[/]");

            AnsiConsole.MarkupLine($"[green]Telegram Bot Token:[/] {MainCore.BotToken}");
            AnsiConsole.MarkupLine($"[green]User Chat ID:[/] {MainCore.userChatID}");
        }

        public static void SetConfigValue(string key, string value)
        {
            try
            {
                Dictionary<string, string> configData;

                if (File.Exists(configFilePath))
                {
                    string existingJson = File.ReadAllText(configFilePath);
                    configData = JsonSerializer.Deserialize<Dictionary<string, string>>(existingJson) ?? new Dictionary<string, string>();
                }
                else
                {
                    configData = new Dictionary<string, string>();
                }

                configData[key] = value;

                string jsonString = JsonSerializer.Serialize(configData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(configFilePath, jsonString);
            }
            catch (Exception ex)
            {
                LoggerService.LogError(ex, "Error in set data config");
            }
        }

        // ذخیره داده جدید در فایل تنظیمات
        public static void SetSettingValue(string key, string value)
        {
            try
            {
                Dictionary<string, string> settingData;

                if (File.Exists(settingFilePath))
                {
                    string existingJson = File.ReadAllText(settingFilePath);
                    settingData = JsonSerializer.Deserialize<Dictionary<string, string>>(existingJson) ?? new Dictionary<string, string>();
                }
                else
                {
                    settingData = new Dictionary<string, string>();
                }

                settingData[key] = value;

                string jsonString = JsonSerializer.Serialize(settingData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(settingFilePath, jsonString);
            }
            catch (Exception ex)
            {
                LoggerService.LogError(ex, "Error in set data setting");
            }
        }
    }
}
