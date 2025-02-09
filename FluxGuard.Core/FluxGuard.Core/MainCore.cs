using Newtonsoft.Json.Linq;
using Service;
using Spectre.Console;
using System;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FluxGuard.Core
{
    public class MainCore
    {
        private int MessageId;
        private readonly string RootPath = AppContext.BaseDirectory;
        public static string BotLanguages;
        public static string BotToken;
        public static long userChatID;
        private static Dictionary<string, int> commands = new Dictionary<string, int>();
        private TelegramBotClient bot;

        public static void Initialize()
        {
            BotToken = DataManager.Config.TelegramBotToken;
            userChatID = long.Parse(DataManager.Config.UserChatId);
            BotLanguages = DataManager.Setting.TelegramBotLanguage;
            Languages.Initialize();
        }
        public bool StartBot()
        {
            try
            {
                bot = new TelegramBotClient(BotToken);
                bot.OnError += OnError;
                bot.OnMessage += OnMessage;
                bot.OnUpdate += OnUpdate;
                LoggerService.LogInformation("MainCore Started");
                return true;
            }
            catch (Exception ex)
            {
                LoggerService.LogError(ex, "StartBot function");
                return false;
            }
        }
        async Task OnError(Exception exception, HandleErrorSource source)
        {
            LoggerService.LogError(exception, "OnError functino");
        }
        async Task OnMessage(Message msg, UpdateType type)
        {
            string? UserName = msg.Chat.Username;
            MessageId = msg.MessageId;

            var stringBuilder = new StringBuilder();
            switch (CommandID(msg.Text))
            {
                //start
                case 0:
                    await StartCommand(userChatID, UserName, stringBuilder);
                    break;
                //back
                case 1:
                    await BackCommand(userChatID, UserName);
                    break;
                //power
                case 2:
                    await PowerCommand(userChatID, UserName);
                    break;
                //status
                case 3:
                    await StatusCommand(userChatID, UserName);
                    break;
                //resource_report
                case 4:
                    await ResourceReportCommand(userChatID, UserName);
                    break;
                //uptime
                case 5:
                    await UpTimeCommand(userChatID, UserName);
                    break;
                //driver_control
                case 6:
                    break;
                //webcam_and_microphone
                case 7:
                    break;

            }
        }

        private async Task UpTimeCommand(long UserID, string UserName)
        {
            LoggerService.LogCommand(UserID, UserName, "uptime");
            await bot.SendMessage(UserID, Services.GetUpTime());
        }

        private async Task ResourceReportCommand(long UserID, string UserName)
        {
            LoggerService.LogCommand(UserID, UserName, "resource_report");
            await bot.SendMessage(UserID, Services.GetSystemUsageReport());
        }

        private async Task StatusCommand(long UserID, string UserName)
        {
            LoggerService.LogCommand(UserID, UserName, "status");
            UI_Manager.StatusDashboard();
            await bot.SendMessage(UserID, Languages.Translate("answers", "status", "status"),
            replyMarkup: UI_Manager.Status_Keyboard);
        }

        private async Task PowerCommand(long UserID, string UserName)
        {
            LoggerService.LogCommand(UserID, UserName, "power");
            Services.TakeScreenShot("Screen");
            Task.Delay(500);
            FileStream stremfile = new FileStream("Screenshot.png", FileMode.Open);
            UI_Manager.PowerDashbord();
            await bot.SendPhoto(UserID, stremfile, replyMarkup: UI_Manager.Power_Keyboard);
            Task.Delay(3000);
            File.Delete("Screenshot.png");
        }

        private async Task BackCommand(long UserID, string UserName)
        {
            LoggerService.LogCommand(UserID, UserName, "back");
            UI_Manager.MainDashboard();
            await bot.SendMessage(UserID, Languages.Translate("answers", "back", "back"),
            replyMarkup: UI_Manager.Main_Keyboard);
        }

        private async Task StartCommand(long UserID, string UserName, StringBuilder stringBuilder)
        {
            LoggerService.LogCommand(UserID, UserName, "start");
            stringBuilder.AppendLine("Welcome to FluxGuard! 🌟\r\nHey there! I'm FluxGuard, your ultimate tool for controlling your computer from anywhere, anytime! 😎 Whether you need to shut down your system, take a screenshot, or keep an eye on your files, I’m here to make it super easy for you! 💻✨\r\n\r\nTo get started, choose your preferred Languagesuage:\r\n1️⃣ English\r\n2️⃣ Persian-فارسی\r\n\r\nLet me know which one you’d prefer, and I’ll be ready to assist you! 🌍🚀");
            var inlineMarkup = new InlineKeyboardMarkup();
            inlineMarkup.AddButton("English", "lan*en");
            inlineMarkup.AddNewRow();
            inlineMarkup.AddButton("فارسی", "lan*fa");
            await bot.SendMessage(UserID, stringBuilder.ToString(),
            replyMarkup: inlineMarkup);
            stringBuilder.Clear();
        }

        async Task OnUpdate(Update update)
        {
            if (update is { CallbackQuery: { } query }) // non-null CallbackQuery
            {
                switch (query.Data)
                {
                    case "shutdown":
                        {
                            await bot.AnswerCallbackQuery(query.Id);
                            await bot.DeleteMessage(query.Message!.Chat, MessageId + 1);
                            await bot.SendMessage(query.Message!.Chat, "سیستم خاموش شد.");
                            await bot.SendMessage(query.Message!.Chat, "دیدار به قیامت");
                            Thread.Sleep(1000);
                            Services.ShutDown();
                            break;
                        }
                    case "sleep":
                        {
                            await bot.AnswerCallbackQuery(query.Id);
                            await bot.DeleteMessage(query.Message!.Chat, MessageId + 1);
                            await bot.SendMessage(query.Message!.Chat, "سیستم به خواب رفت.");
                            await bot.SendMessage(query.Message!.Chat, "شب بخیر");
                            Thread.Sleep(1000);
                            Services.Sleep();
                            break;
                        }
                    case "restart":
                        {
                            await bot.AnswerCallbackQuery(query.Id);
                            await bot.DeleteMessage(query.Message!.Chat, MessageId + 1);
                            await bot.SendMessage(query.Message!.Chat, "سیستم رستارت خواهد شد..");
                            await bot.SendMessage(query.Message!.Chat, "دو دیقه وایسا الان برمیگردم.");
                            Thread.Sleep(1000);
                            Services.Sleep();
                            break;
                        }
                    case "AppsList":
                        {
                            await bot.AnswerCallbackQuery(query.Id);
                            await bot.DeleteMessage(query.Message!.Chat, MessageId + 1);
                            UI_Manager.AppList();
                            var mes = await bot.SendMessage(query.Message!.Chat, "لیست برنامه های باز و قابل کنترل 👇 :",
                            replyMarkup: UI_Manager.App_List);
                            Thread.Sleep(1000);
                            MessageId = MessageId + 1;
                            break;
                        }

                }
                if (query.Data.StartsWith("app*"))
                {
                    await bot.AnswerCallbackQuery(query.Id);
                    string[] nameApp = query.Data.Split("*");
                    Services.TakeScreenShot(nameApp[1]);
                    Thread.Sleep(1500);
                    FileStream streamfile = new FileStream("Screenshot.png", FileMode.Open);
                    InputMediaPhoto AppScreen = new InputMediaPhoto(streamfile);
                    var inlineMarkup = new InlineKeyboardMarkup();
                    inlineMarkup.AddButton("بستن❌", $"Close*{nameApp[1]}");
                    inlineMarkup.AddNewRow();
                    inlineMarkup.AddButton("بازگشت", "AppsList");
                    await bot.EditMessageMediaAsync(query.Message!.Chat, MessageId + 1, AppScreen, replyMarkup: inlineMarkup);
                    Thread.Sleep(4000);
                    File.Delete("Screenshot.png");
                }
                if (query.Data.StartsWith("Close*"))
                {
                    await bot.AnswerCallbackQuery(query.Id);
                    string[] nameApp = query.Data.Split("*");
                    Services.CloseApp(nameApp[1]);
                    await bot.DeleteMessage(query.Message!.Chat, MessageId + 1);
                    await bot.SendMessage(query.Message!.Chat, $"برنامه {nameApp[1]} بسته شد.");

                }
                if (query.Data.StartsWith("lan*"))
                {
                    await bot.AnswerCallbackQuery(query.Id);
                    string[] lan = query.Data.Split("*");
                    BotLanguages = lan[1];
                    Languages.languageCode = BotLanguages;
                    DataManager.SetSettingValue("telegram_bot_language", BotLanguages);
                    await bot.DeleteMessage(query.Message!.Chat, MessageId + 1);
                    UI_Manager.MainDashboard();
                    var mes = await bot.SendMessage(query.Message!.Chat, Languages.Translate("answers", "welcome", "welcome"),
                    replyMarkup: UI_Manager.Main_Keyboard);
                    MessageId = MessageId + 1;
                    LoadLanguagesCommand();
                }
            }
        }
        public int CommandID(string msg)
        {
            if (msg == "/start")
            {
                return 0;
            }
            else
            {
                int command = commands[msg];
                return command;
            }
        }
        public static void LoadLanguagesCommand()
        {
            var commandJson = Languages.language;
            int codeCounter = 1;
            string filePath = $"lan/{BotLanguages}.json";

            string json = File.ReadAllText(filePath);

            var jsonObject = JObject.Parse(json);
            var keyboards = jsonObject["replykeyboards"];

            foreach (var section in keyboards)
            {
                foreach (var key in section.First)
                {
                    commands.Add(((JProperty)key).Value.ToString(), codeCounter++);
                }
            }
        }
    }
}