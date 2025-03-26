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
        // سایر ماژول‌ها

        public static async Task HandelOnMessage(Message msg, UpdateType type)
        {
            switch (CommandID(msg.Text))
            {
                case 0: // start
                    await TelegramClientService.SendMessage("Hi", TelegramKeyboardModel.MainMenuKeyboard,null);
                    break;
                //power
                case 2:
                    message = await TelegramClientService.SendMessage(PowerModule.PowerCommand(),null,TelegramKeyboardModel.PowerOptionsInlineKeyboard);
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
                        await TelegramClientService.SendMessage(PowerModule.ShutdownCommand(), null,null);
                        break;
                    case "sleep":
                        await TelegramClientService.Client.AnswerCallbackQuery(query.Id);
                        await TelegramClientService.DeleteMessage(message);
                        await TelegramClientService.SendMessage(PowerModule.SleepCommand(), null, null);
                        break;
                    case "restart":
                        await TelegramClientService.Client.AnswerCallbackQuery(query.Id);
                        await TelegramClientService.DeleteMessage(message);
                        await TelegramClientService.SendMessage(PowerModule.RestartCommand(), null, null);
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
