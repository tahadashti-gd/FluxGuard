using System.Text;
using FluxGuard.Core.FLuxModules;
using FluxGuard.Core.Models;
using GUI;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FluxGuard.Core.Services
{
    public class CommandHandler
    {
        private static readonly Dictionary<string, int> commands = LanguageService.Commands;
        private static Message? message;

        public static async Task HandelOnMessage(Message msg, UpdateType type)
        {
            switch (CommandID(msg.Text))
            {
                case 0: // start
                    var stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("Welcome to FluxGuard! 🌟\r\nHey there! I'm FluxGuard, your ultimate tool for controlling your computer from anywhere, anytime! 😎 Whether you need to shut down your system, take a screenshot, or keep an eye on your files, I’m here to make it super easy for you! 💻✨\r\n\r\nTo get started, choose your preferred Languagesuage and I’ll be ready to assist you! 🌍🚀");

                    var inlineKeyboard = new List<List<InlineKeyboardButton>>();

                    foreach (var file in LanguageService.GetAvailableLanguages())
                    {
                        inlineKeyboard.Add(new List<InlineKeyboardButton>
                        {
                            InlineKeyboardButton.WithCallbackData(file, $"lan*{file}")
                        });
                    }
                    var inlineMarkup = new InlineKeyboardMarkup(inlineKeyboard);
                    message = await TelegramClientService.SendMessage(stringBuilder.ToString(), null, null, inlineMarkup);
                    break;
                //power
                case 2:
                    message = await TelegramClientService.SendMessage(PowerModule.PowerCommand(), null, null, TelegramKeyboardModel.PowerOptionsInlineKeyboard);
                    break;
                //status
                case 3:
                    await TelegramClientService.SendMessage(StatusModule.StatusCommand(), LanguageService.Translate("answers", "status", "status"), TelegramKeyboardModel.StatusInfoKeyboard, null);
                    break;
                //resource_report
                case 4:
                    await TelegramClientService.SendMessage(StatusModule.GetSystemUsageReport(), null, null, null);
                    break;
                //uptime
                case 5:
                    await TelegramClientService.SendMessage(StatusModule.GetUpTime(), null, null, null);
                    break;
                //driver_control
                case 6:
                    await TelegramClientService.SendMessage(StatusModule.CheckDriversAndDevices(), null, null, null);
                    break;
                //webcam_and_microphone
                case 7:
                    message = await TelegramClientService.SendMessage(StatusModule.GetWebcamAndMicrophoneStatus(), null, null, TelegramKeyboardModel.WebcamMicrophoneInlineKeyboard);
                    break;
                //apps
                case 8:
                    await TelegramClientService.SendMessage(LanguageService.Translate("answers", "apps", "apps"), null, TelegramKeyboardModel.AppMenuKeyboard, null);
                    break;
                //manage_apps
                case 9:
                    message = await TelegramClientService.SendMessage(LanguageService.Translate("replykeyboards", "apps", "manage_apps"), null, null, TelegramKeyboardModel.AppListInlineKeyboard);
                    break;
                //open_app
                case 10:
                    if (ManageAppsModule.GetAvailableApps().Count == 0)
                    {
                        await TelegramClientService.SendMessage(LanguageService.Translate("answers", "apps", "no_apps"), null, null, null);
                    }
                    else
                    {
                        await TelegramClientService.SendMessage(LanguageService.Translate("replykeyboards", "apps", "open_app"), null, null, TelegramKeyboardModel.AppLaunchInlineKeyboard);
                    }
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
            }
        }

        public static async Task HandleOnUpdate(Update update)
        {
            if (update is { CallbackQuery: { } query })
            {
                switch (query.Data)
                {
                    case "shutdown":
                        await TelegramClientService.Client.AnswerCallbackQuery(query.Id);
                        await TelegramClientService.DeleteMessage(message);
                        await TelegramClientService.SendMessage(PowerModule.ShutdownCommand(), null, null, null);
                        break;
                    case "sleep":
                        await TelegramClientService.Client.AnswerCallbackQuery(query.Id);
                        await TelegramClientService.DeleteMessage(message);
                        await TelegramClientService.SendMessage(PowerModule.SleepCommand(), null, null, null);
                        break;
                    case "restart":
                        await TelegramClientService.Client.AnswerCallbackQuery(query.Id);
                        await TelegramClientService.DeleteMessage(message);
                        await TelegramClientService.SendMessage(PowerModule.RestartCommand(), null, null, null);
                        break;
                    case "record_microphone":
                        await TelegramClientService.Client.AnswerCallbackQuery(query.Id);
                        await TelegramClientService.DeleteMessage(message);
                        string microphoneReport;
                        var microphoneRecording = StatusModule.RecordMicrophone(out microphoneReport);
                        await TelegramClientService.SendMessage(LanguageService.Translate("answers", "status", "recording"), null, null, null);
                        await TelegramClientService.SendMessage(microphoneReport, null, null, null);
                        if (microphoneRecording != null)
                        {
                            await TelegramClientService.SendMessage(StatusModule.RecordMicrophone(out microphoneReport), microphoneReport, null, null);
                            microphoneRecording.Dispose();
                        }
                        break;
                    default:
                        if (query.Data.StartsWith("lan*"))
                        {
                            await TelegramClientService.Client.AnswerCallbackQuery(query.Id);
                            string lan = query.Data.Replace("lan*", "");
                            SettingService.SetValue("telegram_bot_language", lan);
                            LanguageService.Initialize();
                            await TelegramClientService.DeleteMessage(message);
                            await TelegramClientService.SendMessage(LanguageService.Translate("answers", "welcome", "welcome"),null, TelegramKeyboardModel.MainMenuKeyboard, null);
                        }
                        else if (query.Data.StartsWith("Mapp*"))
                        {
                            await TelegramClientService.Client.AnswerCallbackQuery(query.Id);
                            string appName = query.Data.Replace("Mapp*", "");
                            LogService.LogServiceActivity("Screenshot: " + appName);
                            ScreenshotService.Screenshot(appName);
                            FileStream streamfile = new FileStream("Screenshot.png", FileMode.Open);
                            InputMediaPhoto AppScreen = new InputMediaPhoto(streamfile);
                            await TelegramClientService.Client.EditMessageMedia(query.Message!.Chat, message.Id, AppScreen, replyMarkup: new InlineKeyboardMarkup().AddButton("❌ " + LanguageService.Translate("inlinekeyboards", "apps", "close"), $"Close*{appName}"));
                        }

                        else if (query.Data.StartsWith("Close*"))
                        {
                            await TelegramClientService.Client.AnswerCallbackQuery(query.Id);
                            string nameApp = query.Data.Replace("Close*", "");
                            await TelegramClientService.DeleteMessage(query.Message);
                            await TelegramClientService.SendMessage(ManageAppsModule.CloseApp(nameApp), null, null, null);
                        }
                        else if (query.Data.StartsWith("launch*"))
                        {
                            string appName = query.Data.Replace("launch*", "");
                            await TelegramClientService.Client.AnswerCallbackQuery(query.Id);
                            await TelegramClientService.SendMessage(LanguageService.Translate("answers", "apps", "launching"), null, null, null);
                            try
                            {
                                await TelegramClientService.SendMessage(ManageAppsModule.LaunchAppAndCaptureScreenshot(appName), string.Format(LanguageService.Translate("answers", "apps", "screenshot_caption"), appName), null, null);
                            }
                            catch (Exception ex)
                            {
                                await TelegramClientService.SendMessage(string.Format(LanguageService.Translate("answers", "apps", "launch_error"), appName, ex.Message), null, null, null);
                            }
                        }
                        break;
                }
            }
        }

        private static int CommandID(string msg)
        {
            if (msg == "/start") return 0;
            return commands.TryGetValue(msg, out var commandId) ? commandId : -1;
        }
    }
}