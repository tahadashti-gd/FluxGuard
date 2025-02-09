using FluxGuard.Core;
using Newtonsoft.Json;
using Serilog;
using Spectre.Console;

namespace FluxGuard.Core
{
    public class Languages
    {
        public static string languageCode;
        public static Dictionary<string, dynamic> language = new Dictionary<string, dynamic>();

        public static void Initialize()
        {
            try
            {
                LoadLanguage(DataManager.Setting.TelegramBotLanguage);
            }
            catch(Exception ex) 
            {
                LoggerService.LogError(ex, "Initializing language");
            }
        }
        public static void LoadLanguage(string langCode)
        {
            try
            {
                string filePath = $"lan/{langCode}.json";
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"Language file not found: {filePath}");

                string jsonContent = File.ReadAllText(filePath);
                LoggerService.LogInformation($"{langCode} Language was successfully loaded.");
                language = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonContent);
                languageCode = langCode;
                MainCore.LoadLanguagesCommand();
            }
            catch (Exception ex)
            {
                LoggerService.LogError(ex, "Load language");
            }
        }
        public static string Translate(string Category, string MainKey, string SubsetKey)
        {
            string result;

            if (language != null)
            {
                result = language[Category][MainKey][SubsetKey];
                return result;
            }
            else
            {
                Log.Error("Error in translating language");
                AnsiConsole.MarkupLine("Error in translating language");
                return "???";
            }
        }
    }
}
