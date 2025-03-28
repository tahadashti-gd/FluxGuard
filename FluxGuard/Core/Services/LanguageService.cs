using Newtonsoft.Json;
using FluxGuard.Core.Models;
using Spectre.Console;
using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace FluxGuard.Core.Services
{
    public class LanguageService
    {
        private static Dictionary<string, dynamic> language = new Dictionary<string, dynamic>();
        public static readonly Dictionary<string, int> Commands = new Dictionary<string, int>();
        private static bool isLanguageLoaded = false;


        public static void Initialize()
        {
            try
            {
                LoadLanguage(SettingModel.Setting.BotLanguage);
                LoadLanguagesCommand(); // لود کردن دستورات بعد از لود زبان
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, "Initializing language");
            }
        }

        public static void LoadLanguage(string langCode)
        {
            try
            {
                //if (isLanguageLoaded && SettingModel.Setting.BotLanguage == langCode && language.Count > 0)
                //{
                //    LogService.LogInformation($"Language '{langCode}' is already loaded. Skipping reload.");
                //    return;
                //}

                string filePath = $"Languages/{langCode}.json";
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"Language file not found: {filePath}");

                string jsonContent = File.ReadAllText(filePath);
                language = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonContent);
                isLanguageLoaded = true;
                LogService.LogInformation($"{langCode} Language was successfully loaded.");
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, "Load language");
                isLanguageLoaded = false;
            }
        }

        public static List<string> GetAvailableLanguages()
        {
            if (Directory.Exists("Languages"))
            {
                List<string> languages = new List<string>();
                languages = new List<string>(Directory.GetFiles("Languages", "*.json"));
                for (int i = 0; i < languages.Count; i++)
                {
                    languages[i] = Path.GetFileNameWithoutExtension(languages[i]);
                }
                return languages;
            }
            else
            {
                return null;
            }
        }

        public static void LoadLanguagesCommand()
        {
            try
            {
                if (!isLanguageLoaded || language == null || !language.ContainsKey("replykeyboards"))
                {
                    LogService.LogError(new Exception("Language data is not loaded or 'replykeyboards' section is missing"), "LoadLanguagesCommand");
                    return;
                }

                Commands.Clear(); // پاک کردن دستورات قبلی
                int codeCounter = 1;

                // استفاده از دیکشنری language به جای خواندن دوباره فایل
                var keyboards = language["replykeyboards"];

                foreach (var section in keyboards)
                {
                    foreach (var key in section.Value)
                    {
                        string commandValue = key.Value.ToString();
                        if (!Commands.ContainsKey(commandValue))
                        {
                            Commands.Add(commandValue, codeCounter++);
                        }
                    }
                }

                LogService.LogInformation($"Loaded {Commands.Count} commands from language file.");
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, "LoadLanguagesCommand");
            }
        }

        public static string Translate(string category, string mainKey, string subsetKey)
        {
            try
            {
                if (language != null && language.ContainsKey(category) && language[category].ContainsKey(mainKey) && language[category][mainKey].ContainsKey(subsetKey))
                {
                    return language[category][mainKey][subsetKey].ToString();
                }
                else
                {
                    LogService.LogError(new Exception($"Translation not found for {category}/{mainKey}/{subsetKey}"), "Translate");
                    AnsiConsole.MarkupLine($"[red]Translation not found: {category}/{mainKey}/{subsetKey}[/]");
                    return "???";
                }
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, "Translate");
                AnsiConsole.MarkupLine($"[red]Error in translating: {category}/{mainKey}/{subsetKey}[/]");
                return "???";
            }
        }
    }
}