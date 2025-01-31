using System.Text;
using Languages;
using Newtonsoft.Json.Linq;
using Service;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FluxGuard.Core
{
    public class MainCore
    {
        int MessageId = 0;
        string RootPath = AppContext.BaseDirectory;
        public static string BotLang = "";
        Dictionary<string, int> commands = new Dictionary<string, int>();
        TelegramBotClient bot = new TelegramBotClient("7675770029:AAFDkAiTQbHbCXx6rbk5xJ1aVfNukbpSa4A");

        public void StartBot()
        {
            bot.OnError += OnError;
            bot.OnMessage += OnMessage;
            bot.OnUpdate += OnUpdate;
        }
        async Task OnError(Exception exception, HandleErrorSource source)
        {
            Console.WriteLine(exception);
        }
        async Task OnMessage(Message msg, UpdateType type)
        {
            var sb = new StringBuilder();
            switch (CommandID(msg.Text))
            {
                //Start
                case 0:
                    sb.AppendLine("Welcome to FluxGuard! 🌟\r\nHey there! I'm FluxGuard, your ultimate tool for controlling your computer from anywhere, anytime! 😎 Whether you need to shut down your system, take a screenshot, or keep an eye on your files, I’m here to make it super easy for you! 💻✨\r\n\r\nTo get started, choose your preferred language:\r\n1️⃣ English\r\n2️⃣ Persian-فارسی\r\n\r\nLet me know which one you’d prefer, and I’ll be ready to assist you! 🌍🚀");
                    var inlineMarkup = new InlineKeyboardMarkup();
                    inlineMarkup.AddButton("English", "lan*en");
                    inlineMarkup.AddNewRow();
                    inlineMarkup.AddButton("فارسی", "lan*fa");
                    await bot.SendMessage(msg.Chat, sb.ToString(),
                    replyMarkup: inlineMarkup);
                    sb.Clear();
                    MessageId = msg.MessageId;
                    break;
                //back
                case 1:
                    UI_Manager.MainDashboard();
                    await bot.SendMessage(msg.Chat, Lang.Translate("answers", "back", "back"),
                    replyMarkup: UI_Manager.Main_Keyboard);
                    break;
                //power
                case 2:
                    Services.TakeScreenShot("Screen");
                    Thread.Sleep(500);
                    FileStream stremfile = new FileStream("Screenshot.png", FileMode.Open);
                    UI_Manager.PowerDashbord();
                    await bot.SendPhoto(msg.Chat, stremfile, replyMarkup: UI_Manager.Power_Keyboard);
                    Thread.Sleep(3000);
                    File.Delete("Screenshot.png");
                    MessageId = msg.MessageId;
                    break;
                //status
                case 3:
                    UI_Manager.StatusDashboard();
                    await bot.SendMessage(msg.Chat, Lang.Translate("answers", "status", "status"),
                    replyMarkup: UI_Manager.Status_Keyboard);
                    break;
                //resource_report
                case 4:
                    await bot.SendMessage(msg.Chat, Services.GetSystemUsageReport());
                    break;

            }
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
                    BotLang = lan[1];
                    Lang.langCode = BotLang;
                    await bot.DeleteMessage(query.Message!.Chat, MessageId + 1);
                    UI_Manager.MainDashboard();
                    var mes = await bot.SendMessage(query.Message!.Chat, Lang.Translate("answers", "welcome", "welcome"),
                    replyMarkup: UI_Manager.Main_Keyboard);
                    Thread.Sleep(1000);
                    MessageId = MessageId + 1;
                    LoadLangCommand();
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
                Console.WriteLine(command);
                return command;
            }
        }
        void LoadLangCommand()
        {
            var commandJson = Lang.LoadLanguage(BotLang);
            int codeCounter = 1;
            string filePath = $"lan/{BotLang}.json";

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
    public static class CoreEvents
    {
        public static Action ShowGUI;
    }

}