using System.Diagnostics;
using System.Runtime.InteropServices;
using FluxGuard.Core.Services;
using GUI;

namespace FluxGuard.Core.FLuxModules
{
    internal class PowerModule
    {
        [DllImport("PowrProf.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

        public static byte[] PowerCommand()
        {
            LogService.LogServiceActivity("Screenshot: Screen");
            ScreenshotService.Screenshot("Screen");
            byte[] imageBytes = File.ReadAllBytes("Screenshot.png");
            return imageBytes;
        }
        public static dynamic ShutdownCommand()
        {
            ShutDown();
            return LanguageService.Translate("answer", "power", "shtudown");
        }
        public static dynamic SleepCommand()
        {
            Sleep();
            return LanguageService.Translate("answer", "power", "sleep");
        }
        public static dynamic RestartCommand()
        {
            Restart();
            return LanguageService.Translate("answer", "power", "restart");
        }

        private static void ShutDown()
        {
            Process.Start("shutdown", "/s /t 0");
            LogService.LogServiceActivity("System shut down");
        }

        private static void Restart()
        {
            Process.Start("shutdown", "/r /t 0");
            LogService.LogServiceActivity("System restarted");
        }

        private static void Sleep()
        {
            SetSuspendState(false, true, true);
            LogService.LogServiceActivity("System put to sleep");
        }
    }
}
