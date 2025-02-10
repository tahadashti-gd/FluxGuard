using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Service;
using Spectre.Console;
using System.IO;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FluxGuard.Core
{
    public class MainCore
    {
        private int MessageId;
        private readonly string RootPath = AppContext.BaseDirectory;
        private static string? LivePath;
        string systemUserName = Environment.UserName;
        public static string? BotLanguages;
        public static string? BotToken;
        public static long userChatID;
        private static Dictionary<string, int> commands = new Dictionary<string, int>();
        private TelegramBotClient bot;

        public static void Initialize()
        {
            try
            {
                BotToken = DataManager.Config.TelegramBotToken;
                userChatID = long.Parse(DataManager.Config.UserChatId);
                BotLanguages = DataManager.Setting.TelegramBotLanguage;
                LoggerService.LogInformation("MainCore Initialize");
                Languages.Initialize();
            }
            catch(Exception ex)
            {
                LoggerService.LogError(ex, "MainCore initializing");
            }
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
                //apps
                case 8:
                    await AppsCommand(userChatID, UserName);
                    break;
                //manage_apps
                case 9:
                    await ManageApps(userChatID,UserName);
                    break;
                //open_app
                case 10:
                    break;
                //send
                case 11:
                    break;
                //send_file
                case 12:
                    break;
                //send_voice
                case 13:
                    break;
                //send_voice
                case 14:
                    break;
                #region file_explorer
                //file_explorer
                case 15:
                    await FileExplorerCommand(userChatID,UserName);
                    break;
                //drive
                case 16:
                    await Drive(userChatID,UserName);
                    MessageId = msg.MessageId;
                    break;
                //desktop
                case 17:
                    LivePath = $@"C:\Users\{systemUserName}\Desktop";
                    await MainFolder(userChatID,UserName, LivePath);
                    MessageId = msg.MessageId;

                    break;
                //download
                case 18:
                    await MainFolder(userChatID, UserName, $@"C:\Users\{systemUserName}\downloads");
                    break;
                //document
                case 19:
                    await MainFolder(userChatID, UserName, $@"C:\Users\{systemUserName}\documents");
                    break;
                //pictures
                case 20:
                    await MainFolder(userChatID, UserName, $@"C:\Users\{systemUserName}\pictures");
                    break;
                //videos
                case 21:
                    await MainFolder(userChatID, UserName, $@"C:\Users\{systemUserName}\videos");
                    break;
                //musics
                case 22:
                    await MainFolder(userChatID, UserName, $@"C:\Users\{systemUserName}\music");
                    break;
                #endregion
                //security
                case 23:
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
        private async Task ManageApps(long UserID, string UserName)
        {
            LoggerService.LogCommand(UserID, UserName, "manage_apps");
            UI_Manager.AppList();
            await bot.SendMessage(UserID,Languages.Translate("replykeyboards","apps", "manage_apps"),replyMarkup:UI_Manager.App_List);
        }
        private async Task StatusCommand(long UserID, string UserName)
        {
            LoggerService.LogCommand(UserID, UserName, "status");
            UI_Manager.StatusDashboard();
            await bot.SendMessage(UserID, Languages.Translate("answers", "status", "status"),
            replyMarkup: UI_Manager.Status_Keyboard);
        }
        private async Task FileExplorerCommand(long UserID, string UserName)
        {
            LoggerService.LogCommand(UserID, UserName, "file_explorer");
            UI_Manager.FileExplorerKeyboard();
            await bot.SendMessage(UserID, Languages.Translate("answers", "file_explorer", "file_explorer"),
            replyMarkup: UI_Manager.FileExplorer_Keyboard);
        }
        private async Task MainFolder(long UserID, string UserName,string path)
        {
            string lastFolder = Path.GetFileName(path);
            LoggerService.LogCommand(UserID, UserName, lastFolder);
            UI_Manager.ShowDirectoryContents(path);
            await bot.SendMessage(UserID, Languages.Translate("replykeyboards", "file_explorer", lastFolder.ToLower()),
            replyMarkup: UI_Manager.Dir_List);
        }
        private async Task LoadDir(long UserID, string UserName, string path)
        {
            string lastFolder = Path.GetFileName(path);
            if (lastFolder == "")
                lastFolder = path;
            LoggerService.LogCommand(UserID, UserName, lastFolder);
            UI_Manager.ShowDirectoryContents(path);
            await bot.EditMessageText(UserID,MessageId+1, lastFolder,
            replyMarkup: UI_Manager.Dir_List);
        }
        private async Task Drive(long UserID, string UserName)
        {
            MessageId = MessageId + 1;
            LoggerService.LogCommand(UserID, UserName, "drive");
            UI_Manager.DriveList();
            await bot.SendMessage(UserID, Languages.Translate("replykeyboards", "file_explorer", "drive"), replyMarkup: UI_Manager.Drive_List);
        }
        private async Task AppsCommand(long UserID, string UserName)
        {
            LoggerService.LogCommand(UserID, UserName, "apps");
            UI_Manager.AppDashboard();
            await bot.SendMessage(UserID, Languages.Translate("answers", "apps", "apps"),
            replyMarkup: UI_Manager.App_Dahsboard);
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
                    case "backDir":
                        await bot.AnswerCallbackQuery(query.Id);
                        DirectoryInfo dirInfo = new DirectoryInfo(LivePath);
                        DirectoryInfo parentDir = dirInfo.Parent;
                        LivePath = parentDir.FullName;
                        LoadDir(userChatID, "", LivePath);
                        break;
                    case "backDrive":
                        await bot.AnswerCallbackQuery(query.Id);
                        await bot.DeleteMessage(userChatID, MessageId + 1);
                        await Drive(userChatID, "UserName");
                        break;
                    case "back_file":
                        LoadDir(userChatID, "", LivePath);
                        break;

                }
                if (query.Data.StartsWith("Mapp*"))
                {
                    await bot.AnswerCallbackQuery(query.Id);
                    string[] nameApp = query.Data.Split("*");
                    Services.TakeScreenShot(nameApp[1]);
                    Thread.Sleep(1500);
                    FileStream streamfile = new FileStream("Screenshot.png", FileMode.Open);
                    InputMediaPhoto AppScreen = new InputMediaPhoto(streamfile);
                    var inlineMarkup = new InlineKeyboardMarkup();
                    inlineMarkup.AddButton("❌"+Languages.Translate("inlinekeyboards","apps","close"), $"Close*{nameApp[1]}");
                    inlineMarkup.AddNewRow();
                    inlineMarkup.AddButton(Languages.Translate("replykeyboards", "back","back"), "AppsList");
                    await bot.EditMessageMedia(query.Message!.Chat, MessageId + 1, AppScreen, replyMarkup: inlineMarkup);
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
                if (query.Data.StartsWith("dir*"))
                {
                    await bot.AnswerCallbackQuery(query.Id);
                    string[] lan = query.Data.Split("*");
                    LivePath = @$"{LivePath}\{lan[1]}";
                    await LoadDir(userChatID, "hgjhg", LivePath);
                }
                if (query.Data.StartsWith("drive*"))
                {
                    await bot.AnswerCallbackQuery(query.Id);
                    string[] lan = query.Data.Split("*");
                    LivePath = lan[1];
                    await LoadDir(userChatID, "hgjhg", LivePath);
                }
                if (query.Data.StartsWith("file*"))
                {
                    await bot.AnswerCallbackQuery(query.Id);
                    string[] lan = query.Data.Split("*");
                    string filePath = @$"{LivePath}\{lan[1]}";
                    FileInfo fileInfo = new FileInfo(filePath);

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"📝 {Languages.Translate("inlinekeyboards", "file_explorer","name")}: {fileInfo.Name}");
                    sb.AppendLine($"📂 {Languages.Translate("inlinekeyboards", "file_explorer", "path")}:");
                    sb.AppendLine(fileInfo.FullName);
                    sb.AppendLine($"📏 {Languages.Translate("inlinekeyboards", "file_explorer", "length")}: {fileInfo.Length / (1024.0 * 1024.0):F2} MB"); // تبدیل به مگابایت
                    sb.AppendLine($"⏳ {Languages.Translate("inlinekeyboards", "file_explorer", "last_edit")}: {fileInfo.LastWriteTime}");

                    var inlineMarkup = new InlineKeyboardMarkup();
                    inlineMarkup.AddButton("📤 "+ Languages.Translate("inlinekeyboards", "file_explorer", "send"), $"send*{lan[1]}");
                    inlineMarkup.AddNewRow();
                    inlineMarkup.AddButton("🗑️ "+ Languages.Translate("inlinekeyboards", "file_explorer","delete"), $"del*{lan[1]}");
                    inlineMarkup.AddNewRow();
                    inlineMarkup.AddButton("⬅️ " + Languages.Translate("replykeyboards", "back", "back"), $"back_file");
                    await bot.EditMessageText(userChatID,MessageId+1,sb.ToString(),replyMarkup:inlineMarkup);

                }
                if (query.Data.StartsWith("send*"))
                {
                    await bot.AnswerCallbackQuery(query.Id);
                    string[] lan = query.Data.Split("*");
                    string filePath = @$"{LivePath}\{lan[1]}";
                    FileStream stremfile = new FileStream(filePath, FileMode.Open);
                    var inlineMarkup = new InlineKeyboardMarkup();
                    await bot.SendDocument(userChatID, stremfile, replyMarkup: inlineMarkup);
                    await bot.DeleteMessage(userChatID,MessageId + 1);

                }
                if (query.Data.StartsWith("del*"))
                {
                    await bot.AnswerCallbackQuery(query.Id);
                    string[] lan = query.Data.Split("*");
                    string filePath = @$"{LivePath}\{lan[1]}";
                    File.Delete(filePath);
                    await bot.DeleteMessage(userChatID, MessageId + 1);
                    await bot.SendMessage(query.Message!.Chat, lan[1] +Languages.Translate("answers", "file_explorer", "delete"));
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