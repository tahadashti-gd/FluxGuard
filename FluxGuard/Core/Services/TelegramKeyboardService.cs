using FluxGuard.Core.FLuxModules;
using FluxGuard.Core.Models;
using Spectre.Console;
using Telegram.Bot.Types.ReplyMarkups;

namespace FluxGuard.Core.Services
{
    internal class TelegramKeyboardService
    {
        public static ReplyKeyboardMarkup InitializeMainMenuKeyboard()
        {
            ReplyKeyboardMarkup MainMenuKeyboard = new ReplyKeyboardMarkup("");
            KeyboardButton[] row1 = { new(LanguageService.Translate("replykeyboards", "status", "status")), new(LanguageService.Translate("replykeyboards", "power", "power")) };
            KeyboardButton[] row2 = { new(LanguageService.Translate("replykeyboards", "apps", "apps")), new(LanguageService.Translate("replykeyboards", "send", "send")) };
            KeyboardButton[] row3 = { new(LanguageService.Translate("replykeyboards", "file_explorer", "file_explorer")) };
            KeyboardButton[] row4 = { new(LanguageService.Translate("replykeyboards", "security", "security")) };
            KeyboardButton[] row5 = { new(LanguageService.Translate("replykeyboards", "settings", "settings")) };
            MainMenuKeyboard = new[] { row1, row2, row3, row4, row5 };
            LogService.LogUI("Main menu keyboard initialized");
            return MainMenuKeyboard;
        }

        public static ReplyKeyboardMarkup InitializeAppMenuKeyboard()
        {
            ReplyKeyboardMarkup appMenuKeyboard = new ReplyKeyboardMarkup("");
            KeyboardButton[] row0 = { new(LanguageService.Translate("replykeyboards", "apps", "manage_apps")) };
            KeyboardButton[] row1 = { new(LanguageService.Translate("replykeyboards", "apps", "open_app")) };
            KeyboardButton[] row2 = { new(LanguageService.Translate("replykeyboards", "back", "back")) };
            appMenuKeyboard.Keyboard = new[] { row0, row1, row2 };
            LogService.LogUI("App menu keyboard initialized");
            return appMenuKeyboard;
        }

        public static ReplyKeyboardMarkup InitializeStatusMenuKeyboard()
        {
            ReplyKeyboardMarkup statusInfoKeyboard = new ReplyKeyboardMarkup("");
            KeyboardButton[] row0 = { new(LanguageService.Translate("replykeyboards", "status", "resource_report")), new(LanguageService.Translate("replykeyboards", "status", "driver_control")) };
            KeyboardButton[] row1 = { new(LanguageService.Translate("replykeyboards", "status", "uptime")), new(LanguageService.Translate("replykeyboards", "status", "webcam_and_microphone")) };
            KeyboardButton[] row2 = { new(LanguageService.Translate("replykeyboards", "back", "back")) };
            statusInfoKeyboard.Keyboard = new[] { row0, row1, row2 };
            LogService.LogUI("Status menu keyboard initialized");
            return statusInfoKeyboard;
        }

        public static InlineKeyboardMarkup InitializePowerOptionsInlineKeyboard()
        {
            var inlineMarkup = new InlineKeyboardMarkup()
                .AddButton("⭕" + LanguageService.Translate("inlinekeyboards", "power", "shutdown"), "shutdown")
                .AddNewRow()
                .AddButton("💤" + LanguageService.Translate("inlinekeyboards", "power", "sleep"), "sleep")
                .AddButton("🔄️" + LanguageService.Translate("inlinekeyboards", "power", "restart"), "restart");
            LogService.LogUI("Power options inline keyboard initialized");
            return inlineMarkup;
        }

        public static InlineKeyboardMarkup InitializeAppListInlineKeyboard()
        {
            var inlineMarkup = new InlineKeyboardMarkup();
            foreach (var app in ManageAppsModule.GetAllWindowHandleNames())
            {
                inlineMarkup.AddButton(app, "Mapp*" + app);
                inlineMarkup.AddNewRow();
            }
            LogService.LogUI("App list inline keyboard initialized");
            return inlineMarkup;
        }

        public static InlineKeyboardMarkup InitializeDriveListInlineKeyboard()
        {
            var inlineMarkup = new InlineKeyboardMarkup();
            foreach (var drive in FileExplorerModule.GetDrives())
            {
                inlineMarkup.AddButton(drive, "drive*" + drive);
                inlineMarkup.AddNewRow();
            }
            LogService.LogUI("Drive list inline keyboard initialized");
            return inlineMarkup;
        }

        public static InlineKeyboardMarkup InitializeDirectoryListInlineKeyboard(string path)
        {
            string lastFolder = Path.GetFileName(path);
            var inlineMarkup = new InlineKeyboardMarkup();
            foreach (var folder in FileExplorerModule.GetDirectories(path))
            {
                inlineMarkup.AddButton("📁 " + ShortenName(folder.Name), "d*" + ShortenName(folder.Name));
                inlineMarkup.AddNewRow();
            }
            foreach (var file in FileExplorerModule.GetFiles(path))
            {
                inlineMarkup.AddButton("📄 " + ShortenName(file.Name), "f*" + ShortenName(file.Name));
                inlineMarkup.AddNewRow();
            }
            bool isRootDrive = DriveInfo.GetDrives().Any(d => d.Name.TrimEnd('\\') == path.TrimEnd('\\'));
            inlineMarkup.AddButton("⬅️ " + LanguageService.Translate("replykeyboards", "back", "back"), isRootDrive ? "backDrive" : "backDir");
            LogService.LogUI($"Directory list inline keyboard initialized for path: {path}");
            return inlineMarkup;
        }

        public static ReplyKeyboardMarkup InitializeFileExplorerMenuKeyboard()
        {
            try
            {
                ReplyKeyboardMarkup fileExplorerMenuKeyboard = new ReplyKeyboardMarkup("");
                KeyboardButton[] row01 = { new(LanguageService.Translate("replykeyboards", "file_explorer", "drive")) };
                KeyboardButton[] row02 = { new(LanguageService.Translate("replykeyboards", "file_explorer", "desktop")), new(LanguageService.Translate("replykeyboards", "file_explorer", "download")) };
                KeyboardButton[] row03 = { new(LanguageService.Translate("replykeyboards", "file_explorer", "musics")), new(LanguageService.Translate("replykeyboards", "file_explorer", "document")) };
                KeyboardButton[] row04 = { new(LanguageService.Translate("replykeyboards", "file_explorer", "pictures")), new(LanguageService.Translate("replykeyboards", "file_explorer", "videos")) };
                KeyboardButton[] row05 = { new(LanguageService.Translate("replykeyboards", "back", "back")) };
                fileExplorerMenuKeyboard.Keyboard = new[] { row01, row02, row03, row04, row05 };
                LogService.LogUI("File explorer menu keyboard initialized");
                return fileExplorerMenuKeyboard;
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, "Initialize file explorer menu keyboard");
                AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
                return new ReplyKeyboardMarkup();
            }
        }

        private static string ShortenName(string name) => name.Length > 20 ? name.Substring(0, 17) + "..." : name;
    }
}