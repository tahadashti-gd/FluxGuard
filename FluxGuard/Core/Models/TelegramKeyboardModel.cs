using FluxGuard.Core.Services;
using Telegram.Bot.Types.ReplyMarkups;

namespace FluxGuard.Core.Models
{
    internal class TelegramKeyboardModel
    {
        /// <summary>
        /// Represents the main menu keyboard with options like Status, Power, Apps, etc.
        /// </summary>
        public static ReplyKeyboardMarkup MainMenuKeyboard
        {
            get { return TelegramKeyboardService.InitializeMainMenuKeyboard(); }
        }

        /// <summary>
        /// Represents the application management menu keyboard with options to manage/open apps.
        /// </summary>
        public static ReplyKeyboardMarkup AppMenuKeyboard
        {
            get { return TelegramKeyboardService.InitializeAppMenuKeyboard(); }
        }

        /// <summary>
        /// Inline keyboard markup displaying a list of running applications with interactive buttons.
        /// </summary>
        public static InlineKeyboardMarkup AppListInlineKeyboard
        {
            get { return TelegramKeyboardService.InitializeAppListInlineKeyboard(); }
        }

        /// <summary>
        /// Inline keyboard markup displaying available drives on the system for navigation.
        /// </summary>
        public static InlineKeyboardMarkup DriveListInlineKeyboard
        {
            get { return TelegramKeyboardService.InitializeDriveListInlineKeyboard(); }
        }

        /// <summary>
        /// Inline keyboard markup showing directory contents (folders and files) for file exploration.
        /// </summary>
        public static InlineKeyboardMarkup DirectoryListInlineKeyboard
        {
            get { return TelegramKeyboardService.InitializeDirectoryListInlineKeyboard(""); }
        }

        /// <summary>
        /// Inline keyboard with power options like Shutdown, Restart, and Sleep.
        /// </summary>
        public static InlineKeyboardMarkup PowerOptionsInlineKeyboard
        {
            get { return TelegramKeyboardService.InitializePowerOptionsInlineKeyboard(); }
        }

        /// <summary>
        /// Keyboard for system status information and controls like resource monitoring and driver control.
        /// </summary>
        public static ReplyKeyboardMarkup StatusInfoKeyboard
        {
            get { return TelegramKeyboardService.InitializeStatusMenuKeyboard(); }
        }

        /// <summary>
        /// Keyboard for file explorer navigation with quick access to common directories.
        /// </summary>
        public static ReplyKeyboardMarkup FileExplorerMenuKeyboard
        {
            get { return TelegramKeyboardService.InitializeFileExplorerMenuKeyboard(); }
        }
    }
}