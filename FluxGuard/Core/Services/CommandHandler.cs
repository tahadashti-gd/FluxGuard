using FluxGuard.Core.FLuxModules;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using FluxGuard.Core.Models;

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
                    await TelegramClientService.SendMessage("Hi",null, TelegramKeyboardModel.MainMenuKeyboard,null);
                    break;
                //power
                case 2:
                    message = await TelegramClientService.SendMessage(PowerModule.PowerCommand(),null,null,TelegramKeyboardModel.PowerOptionsInlineKeyboard);
                    break;
                //status
                case 3:
                    await TelegramClientService.SendMessage(StatusModule.StatusCommand(), LanguageService.Translate("answers", "status", "status"),TelegramKeyboardModel.StatusInfoKeyboard,null);
                    break;
                //resource_report
                case 4:
                    await TelegramClientService.SendMessage(StatusModule.GetSystemUsageReport(), null, null,null);
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
                        await TelegramClientService.SendMessage(PowerModule.ShutdownCommand(),null, null,null);
                        break;
                    case "sleep":
                        await TelegramClientService.Client.AnswerCallbackQuery(query.Id);
                        await TelegramClientService.DeleteMessage(message);
                        await TelegramClientService.SendMessage(PowerModule.SleepCommand(),null, null, null);
                        break;
                    case "restart":
                        await TelegramClientService.Client.AnswerCallbackQuery(query.Id);
                        await TelegramClientService.DeleteMessage(message);
                        await TelegramClientService.SendMessage(PowerModule.RestartCommand(),null, null, null);
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
