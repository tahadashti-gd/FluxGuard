using System.Text.Json.Serialization;

namespace FluxGuard.Core.Models
{
    public class ConfigModel
    {
        // File paths for config and settings
        public static string FilePath = "config.json";

        // Public properties to access config and settings
        public static Model Config { get; set; }

        // Model for configuration
        public class Model
        {
            [JsonPropertyName("telegram_bot_token")]
            public string? TelegramBotToken { get; set; } = "";

            [JsonPropertyName("user_chat_id")]
            public long UserChatId { get; set; }
        }
    }
}
