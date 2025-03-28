using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FluxGuard.Core.Services;
using GUI;

namespace FluxGuard.Core.FLuxModules
{
    public class ManageAppsModule
    {
        private const string AppsDirectory = "apps";

        public static List<string> GetAllWindowHandleNames()
        {
            List<string> windowHandleNames = new();
            foreach (Process window in Process.GetProcesses())
            {
                window.Refresh();
                if (window.MainWindowHandle != IntPtr.Zero && !string.IsNullOrEmpty(window.MainWindowTitle))
                    windowHandleNames.Add(window.ProcessName);
            }
            LogService.LogServiceActivity("Fetched all window handle names");
            return windowHandleNames;
        }

        public static string CloseApp(string nameWindow)
        {
            Process me = Process.GetCurrentProcess();
            try
            {
                foreach (Process p in Process.GetProcesses())
                {
                    if (p.Id != me.Id && p.ProcessName == nameWindow)
                        p.Kill();
                }
                LogService.LogServiceActivity($"Closed app: {nameWindow}");

                string appClosedMessage = string.Format(LanguageService.Translate("answers", "apps", "app_closed"), nameWindow);
                return appClosedMessage;
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, $"Failed to close app: {nameWindow}");
                return string.Empty;
            }
        }

        public static List<string> GetAvailableApps()
        {
            string appsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppsDirectory);
            if (!Directory.Exists(appsPath))
            {
                Directory.CreateDirectory(appsPath);
                LogService.LogServiceActivity("Created apps directory");
            }

            var shortcutFiles = Directory.GetFiles(appsPath, "*.lnk");
            return shortcutFiles.Select(Path.GetFileNameWithoutExtension).ToList();
        }

        public static byte[] LaunchAppAndCaptureScreenshot(string appName)
        {
            string shortcutPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppsDirectory, $"{appName}.lnk");

            if (!File.Exists(shortcutPath))
            {
                LogService.LogWarning("LaunchApp", $"Shortcut not found: {shortcutPath}");
                throw new FileNotFoundException($"Shortcut not found for {appName}");
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = shortcutPath,
                    UseShellExecute = true
                });

                Thread.Sleep(4000);

                LogService.LogServiceActivity("Screenshot: "+appName);
                ScreenshotService.Screenshot(appName);
                byte[] imageBytes = File.ReadAllBytes("Screenshot.png");
                return imageBytes;

            }
            catch (Exception ex)
            {
                LogService.LogError(ex, $"Failed to launch app or capture screenshot: {appName}");
                throw;
            }
        }
    }
}