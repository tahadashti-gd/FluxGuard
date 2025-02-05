using Newtonsoft.Json;
using Serilog;

namespace Languages
{
    public class Lang
    {
        public static string langCode = "en";

        public static Dictionary<string, dynamic> LoadLanguage(string langCode)
        {
            try
            {
                string filePath = $"lan/{langCode}.json";
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"Language file not found: {filePath}");

                string jsonContent = File.ReadAllText(filePath);
                Log.Information($"{langCode} language was successfully loaded.");
                return JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonContent);
            }
            catch (Exception ex)
            {
                Log.Error($"Error loading language file: {ex.Message}");
                return null;
            }
        }

        public static string Translate(string Category, string MainKey, string SubsetKey)
        {
            string result;

            var translations = LoadLanguage(langCode);

            if (translations != null)
            {
                result = translations[Category][MainKey][SubsetKey];
                return result;
            }
            else
            {
                Log.Error("Error in text translation");
                return "Error";
            }
        }
    }
}
