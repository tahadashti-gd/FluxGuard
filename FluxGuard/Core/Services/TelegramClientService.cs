using Telegram.Bot;
using Telegram.Bot.Types;
using FluxGuard.Core.Models;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FluxGuard.Core.Services
{
    internal class TelegramClientService
    {
        public static TelegramBotClient Client;
        private static long chatId;

        public bool StartBot()
        {
            try
            {
                Client = new TelegramBotClient(ConfigModel.Config.TelegramBotToken);
                Client.OnError += OnError;
                Client.OnMessage += OnMessage;
                Client.OnUpdate += OnUpdate;
                chatId = ConfigModel.Config.UserChatId;
                LogService.LogInformation("Telegram client started");
                return true;
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, "StartBot");
                return false;
            }
        }

        async Task OnError(Exception exception, HandleErrorSource source)
        {
            LogService.LogError(exception, "Telegram client error");
        }

        private static async Task OnMessage(Message msg, UpdateType type)
        {
            LogService.LogInformation($"Message received: {msg.Text}");
            await CommandHandler.HandelOnMessage(msg, type);
        }

        private static async Task OnUpdate(Update update)
        {
            LogService.LogInformation("Update received");
            await CommandHandler.HandleOnUpdate(update);
        }

        public static async Task<Message> SendMessage(object content, ReplyKeyboardMarkup? replyKeyboard, InlineKeyboardMarkup? inlineKeyboard)
        {
            Message message;
            try
            {
                ReplyMarkup? markup = replyKeyboard != null ? replyKeyboard : inlineKeyboard;

                switch (content)
                {
                    case string text:
                        message = await Client.SendMessage(chatId, text, replyMarkup: markup);
                        LogService.LogBotResponse(text, chatId);
                        break;

                    case byte[] fileBytes:
                        message = await Client.SendPhoto(chatId, new InputFileStream(new MemoryStream(fileBytes)), replyMarkup: markup);
                        LogService.LogBotResponse("Photo sent", chatId);
                        break;

                    case Stream fileStream:
                        message = await Client.SendDocument(chatId, new InputFileStream(fileStream), replyMarkup: markup);
                        LogService.LogBotResponse("Document sent", chatId);
                        break;

                    case InputFile inputFile:
                        message = await Client.SendPhoto(chatId, inputFile, replyMarkup: markup);
                        LogService.LogBotResponse("File sent", chatId);
                        break;

                    default:
                        LogService.LogWarning("SendMessage", $"Unsupported content type: {content.GetType().Name}");
                        throw new ArgumentException("Unsupported content type");
                }
                LogService.LogInformation($"Content sent to chat {chatId}");
                return message;

            }
            catch (Exception ex)
            {
                LogService.LogError(ex, $"Failed to send content to chat {chatId}");
                throw;
            }
        }
    
        public static async Task DeleteMessage(Message message)
        {
            await Client.DeleteMessage(ConfigModel.Config.UserChatId,message.Id);
        }
    }
}