using FluxGuard.Core.Services;
using Telegram.Bot.Types.ReplyMarkups;

namespace FluxGuard.Core.Models
{
    internal class TelegramKeyboardModel
    {
        public static ReplyKeyboardMarkup MainMenuKeyboard
        {
            get { return TelegramKeyboardService.InitializeMainMenuKeyboard(); }
        }

        public static ReplyKeyboardMarkup AppMenuKeyboard
        {
            get { return TelegramKeyboardService.InitializeAppMenuKeyboard(); }
        }

        public static InlineKeyboardMarkup AppListInlineKeyboard
        {
            get { return TelegramKeyboardService.InitializeAppListInlineKeyboard(); }
        }

        public static InlineKeyboardMarkup AppLaunchInlineKeyboard // اضافه شده
        {
            get { return TelegramKeyboardService.InitializeAppLaunchInlineKeyboard(); }
        }

        public static InlineKeyboardMarkup DriveListInlineKeyboard
        {
            get { return TelegramKeyboardService.InitializeDriveListInlineKeyboard(); }
        }

        public static InlineKeyboardMarkup DirectoryListInlineKeyboard
        {
            get { return TelegramKeyboardService.InitializeDirectoryListInlineKeyboard(""); }
        }

        public static InlineKeyboardMarkup PowerOptionsInlineKeyboard
        {
            get { return TelegramKeyboardService.InitializePowerOptionsInlineKeyboard(); }
        }

        public static ReplyKeyboardMarkup StatusInfoKeyboard
        {
            get { return TelegramKeyboardService.InitializeStatusMenuKeyboard(); }
        }

        public static ReplyKeyboardMarkup FileExplorerMenuKeyboard
        {
            get { return TelegramKeyboardService.InitializeFileExplorerMenuKeyboard(); }
        }
        public static InlineKeyboardMarkup WebcamMicrophoneInlineKeyboard
        {
            get { return TelegramKeyboardService.InitializeWebcamMicrophoneInlineKeyboard(); }
        }
    }
}