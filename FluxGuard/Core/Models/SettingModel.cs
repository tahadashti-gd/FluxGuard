using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FluxGuard.Core.Models
{
    internal class SettingModel
    {
        // File paths for settings
        public static string FilePath = "setting.json";

        // Public properties to access settings
        public static Model Setting { get; set; }

        // Model for application settings
        public class Model
        {
            [JsonPropertyName("telegram_bot_language")]
            public string? BotLanguage { get; set; } = "en";

            [JsonPropertyName("automatic_start")]
            public bool AutomaticStart { get; set; } = false;
        }
    }
}
