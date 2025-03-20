using NUnit.Framework;
using System.IO;
using System;
using Moq;
using FluxGuard.Core;

namespace FluxGuard.Tests
{
    [TestFixture]
    public class FluxGuardTests
    {
        private string configFilePath = "config.json";
        private string settingFilePath = "setting.json";
        private string logFilePath => $"logs/fluxguard{DateTime.Now:yyyyMMdd}.log";

        [SetUp]
        public void Setup()
        {
            // آماده‌سازی محیط تست
            Directory.CreateDirectory("logs");
            File.WriteAllText(configFilePath, "{\"telegram_bot_token\":\"test_token\",\"user_chat_id\":\"12345\"}");
            File.WriteAllText(settingFilePath, "{\"telegram_bot_language\":\"en\",\"automatic_start\":true}");
            DataManager.Initialize();
            Logger.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            // پاکسازی بعد از تست
            if (File.Exists(configFilePath)) File.Delete(configFilePath);
            if (File.Exists(settingFilePath)) File.Delete(settingFilePath);
            if (File.Exists(logFilePath)) File.Delete(logFilePath);
            if (Directory.Exists("logs")) Directory.Delete("logs", true);
        }

        // تست‌های DataManager
        [Test]
        public void DataManager_LoadConfig_LoadsCorrectly()
        {
            DataManager.LoadConfig();
            Assert.IsNotNull(DataManager.Config);
            Assert.AreEqual("test_token", DataManager.Config.TelegramBotToken);
            Assert.AreEqual("12345", DataManager.Config.UserChatId);
        }

        [Test]
        public void DataManager_SetConfigValue_UpdatesConfigFile()
        {
            DataManager.SetConfigValue("telegram_bot_token", "new_token");
            DataManager.LoadConfig();
            Assert.AreEqual("new_token", DataManager.Config.TelegramBotToken);
        }

        // تست‌های Logger
        [Test]
        public void Logger_LogInformation_WritesToFile()
        {
            LoggerService.LogInformation("Test log entry");
            Assert.IsTrue(File.Exists(logFilePath));
            string logContent = File.ReadAllText(logFilePath);
            Assert.IsTrue(logContent.Contains("Test log entry"));
        }

        [Test]
        public void Logger_ShowRecentLogs_ReturnsFilteredLogs()
        {
            LoggerService.LogInformation("Old log");
            System.Threading.Thread.Sleep(1000); // صبر برای اختلاف زمانی
            LoggerService.LogInformation("Recent log");
            Logger.ShowRecentLogs(1); // فقط لاگ‌های 1 ساعت اخیر
            string consoleOutput = Console.Out.ToString();
            Assert.IsTrue(consoleOutput.Contains("Recent log"));
            Assert.IsFalse(consoleOutput.Contains("Old log"));
        }

        // تست‌های MainCore
        [Test]
        public void MainCore_CommandID_ReturnsZeroForStart()
        {
            var mainCore = new MainCore();
            int commandId = mainCore.CommandID("/start");
            Assert.AreEqual(0, commandId);
        }

        // تست‌های Services
        //[Test]
        //public void Services_GetUpTime_ReturnsValidFormat()
        //{
            
        //    string uptime = Services.GetUpTime();
        //    Assert.IsNotNull(uptime);
        //    Assert.IsTrue(uptime.Contains(":")); // باید فرمت زمان داشته باشه
        //}

        //[Test]
        //public void Services_GetDrives_ReturnsNonEmptyList()
        //{
        //    var drives = Services.GetDrives();
        //    Assert.IsNotNull(drives);
        //    Assert.IsTrue(drives.Count > 0);
        //}

        //[Test]
        //public void Services_GetSystemUsageReport_ContainsExpectedData()
        //{
        //    string report = Services.GetSystemUsageReport();
        //    Assert.IsTrue(report.Contains("CPU"));
        //    Assert.IsTrue(report.Contains("RAM"));
        //    Assert.IsTrue(report.Contains("GPU"));
        //}

        //// تست‌های UI_Manager
        //[Test]
        //public void UI_Manager_MainDashboard_CreatesKeyboard()
        //{
        //    Languages.Initialize(); // فرض بر اینکه زبان بارگذاری شده
        //    UI_Manager.MainDashboard();
        //    Assert.IsNotNull(UI_Manager.Main_Keyboard);
        //    Assert.IsTrue(UI_Manager.Main_Keyboard.Keyboard.Length > 0);
        //}

        //[Test]
        //public void UI_Manager_DriveList_PopulatesDrives()
        //{
        //    UI_Manager.DriveList();
        //    Assert.IsNotNull(UI_Manager.Drive_List);
        //    Assert.IsTrue(UI_Manager.Drive_List.InlineKeyboard.Length > 0);
        //}
    }
}