using Service;
using Spectre.Console;
using Telegram.Bot.Types.ReplyMarkups;

namespace FluxGuard.Core
{
    public class UI_Manager
    {
        public static ReplyKeyboardMarkup Main_Keyboard;
        public static ReplyKeyboardMarkup Files_Keyboard;
        public static ReplyKeyboardMarkup Get_Keyboard;
        public static ReplyKeyboardMarkup App_Dahsboard;
        public static InlineKeyboardMarkup App_List;
        public static InlineKeyboardMarkup Drive_List;
        public static InlineKeyboardMarkup Dir_List;
        public static InlineKeyboardMarkup Power_Keyboard;
        public static ReplyKeyboardMarkup Status_Keyboard;
        public static ReplyKeyboardMarkup FileExplorer_Keyboard;
        public static ReplyKeyboardMarkup MainFolder_Keyboard;
        public static InlineKeyboardMarkup inline_Keyboard;
        //private ReplyKeyboardMarkup rkm;

        public static void MainDashboard()
        {
            Main_Keyboard = new ReplyKeyboardMarkup("");
            KeyboardButton[] row1 =
            {
                new KeyboardButton(Languages.Translate("replykeyboards","status","status")),new KeyboardButton(Languages.Translate("replykeyboards","power","power"))
            };
            KeyboardButton[] row2 =
            {
                new KeyboardButton(Languages.Translate("replykeyboards","apps","apps")),new KeyboardButton(Languages.Translate("replykeyboards","send","send"))
            };
            KeyboardButton[] row3 =
            {
                new KeyboardButton(Languages.Translate("replykeyboards","file_explorer","file_explorer"))
            };
            KeyboardButton[] row4 =
            {
                new KeyboardButton(Languages.Translate("replykeyboards","security","security"))
            };
            KeyboardButton[] row5 =
            {
                new KeyboardButton(Languages.Translate("replykeyboards","settings","settings"))
            };
            Main_Keyboard.Keyboard = new KeyboardButton[][]
            {
                row1 , row2 , row3 , row4 , row5
            };
            LoggerService.LogUI("MainDashboard Loaded.");

        }
        public static void AppDashboard()
        {
            App_Dahsboard = new ReplyKeyboardMarkup("");
            KeyboardButton[] row0 =
            {
                 new KeyboardButton(Languages.Translate("replykeyboards","apps","manage_apps"))
            };
            KeyboardButton[] row1 =
            {
                 new KeyboardButton(Languages.Translate("replykeyboards","apps","open_app"))
            };
            KeyboardButton[] row2 =
            {
                 new KeyboardButton(Languages.Translate("replykeyboards","back","back"))
            };
            App_Dahsboard.Keyboard = new KeyboardButton[][]
            {
                 row0,row1,row2
            };
        }
        public static void StatusDashboard()
        {
            Status_Keyboard = new ReplyKeyboardMarkup("");
            KeyboardButton[] row0 =
            {
                 new KeyboardButton(Languages.Translate("replykeyboards","status","resource_report")),new KeyboardButton(Languages.Translate("replykeyboards","status","driver_control"))
            };
            KeyboardButton[] row1 =
            {
                 new KeyboardButton(Languages.Translate("replykeyboards","status","uptime")),new KeyboardButton(Languages.Translate("replykeyboards","status","webcam_and_microphone"))
            };
            KeyboardButton[] row2 =
            {
                 new KeyboardButton(Languages.Translate("replykeyboards","back","back"))
            };
            Status_Keyboard.Keyboard = new KeyboardButton[][]
            {
                  row0,row1,row2
            };
            LoggerService.LogUI("StatusDashboard Loaded.");

        }
        public static void PowerDashbord()
        {

            var inlineMarkup = new InlineKeyboardMarkup()
            .AddButton("⭕" + Languages.Translate("inlinekeyboards", "power", "shutdown"), "shutdown") // first row, first button
            .AddNewRow()
            .AddButton("💤" + Languages.Translate("inlinekeyboards", "power", "sleep"), "sleep") // second row, first button
            .AddButton("🔄️" + Languages.Translate("inlinekeyboards", "power", "restart"), "restart");// second row, second button

            Power_Keyboard = inlineMarkup;

        }
        public static void AppList()
        {
            var inlineMarkup = new InlineKeyboardMarkup();
            foreach (var apps in Services.GetAllWindowHandleNames())
            {
                inlineMarkup.AddButton(apps, "Mapp*" + apps);
                inlineMarkup.AddNewRow();
            }
            App_List = inlineMarkup;
        }
        public static void DriveList()
        {
            var inlineMarkup = new InlineKeyboardMarkup();
            foreach (var drive in Services.GetDrives())
            {
                inlineMarkup.AddButton(drive, "drive*" + drive);
                inlineMarkup.AddNewRow();
            }
            Drive_List = inlineMarkup;
        }
        public static void ShowDirectoryContents(string path)
        {
            string lastFolder = Path.GetFileName(path);
            var inlineMarkup = new InlineKeyboardMarkup();
            foreach (var folder in Services.GetDirectories(path))
            {
                inlineMarkup.AddButton("📁 " + Services.ShortenName(folder.Name), "dir*" + Services.ShortenName(folder.Name));
                inlineMarkup.AddNewRow();
            }
            foreach (var file in Services.GetFiles(path))
            {
                inlineMarkup.AddButton("📄 " + Services.ShortenName(file.Name), "file*" + Services.ShortenName(file.Name));
                inlineMarkup.AddNewRow();
            }
            bool isRootDrive = DriveInfo.GetDrives().Any(d => d.Name.TrimEnd('\\') == path.TrimEnd('\\'));

            if (isRootDrive)
                inlineMarkup.AddButton("⬅️ " + Languages.Translate("replykeyboards", "back", "back"), $"backDrive");
            else
                inlineMarkup.AddButton("⬅️ " + Languages.Translate("replykeyboards", "back", "back"), $"backDir");
            Dir_List = inlineMarkup;
        }
        public static void FileExplorerKeyboard()
        {
            try
            {
                FileExplorer_Keyboard = new ReplyKeyboardMarkup("");
                KeyboardButton[] row01 =
                {
                                    new KeyboardButton(Languages.Translate("replykeyboards","file_explorer","drive"))
                                };
                KeyboardButton[] row02 =
                {
                                    new KeyboardButton(Languages.Translate("replykeyboards","file_explorer","desktop")),new KeyboardButton(Languages.Translate("replykeyboards","file_explorer","download"))
                                };
                KeyboardButton[] row03 =
               {
                                    new KeyboardButton(Languages.Translate("replykeyboards","file_explorer","musics")),new KeyboardButton(Languages.Translate("replykeyboards","file_explorer","document"))
                                };
                KeyboardButton[] row04 =
               {
                                    new KeyboardButton(Languages.Translate("replykeyboards","file_explorer","pictures")) , new KeyboardButton(Languages.Translate("replykeyboards","file_explorer","videos"))
                                };
                KeyboardButton[] row05 =
                {
                                    new KeyboardButton(Languages.Translate("replykeyboards","back","back"))
                                };
                FileExplorer_Keyboard.Keyboard = new KeyboardButton[][]
                {
                 row01,row02,row03,row04,row05
                };
            }
            catch (Exception ex)
            {
                AnsiConsole.Markup(ex.Message);
            }
        }

    }
}
