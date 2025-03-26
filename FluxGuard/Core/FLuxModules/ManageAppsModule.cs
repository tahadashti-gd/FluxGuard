using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluxGuard.Core.FLuxModules
{
    public class ManageAppsModule
    {
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

        public static void CloseApp(string nameWindow)
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
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, $"Failed to close app: {nameWindow}");
            }
        }
    }
}
