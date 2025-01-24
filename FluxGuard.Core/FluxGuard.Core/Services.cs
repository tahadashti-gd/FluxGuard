using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace Service
{
    internal static class Services
    {
        #region Import dll'ls

        [DllImport("PowrProf.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        private static int WHKEYBOARDLL = 13;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

        [DllImport("user32.dll")]
        static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int destIp, int srcIP, byte[] macAddr, ref uint physicalAddrLen);

        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)]
        private static extern IntPtr SendMessageW(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("Winmm.dll", SetLastError = true)]
        static extern int mciSendString(string lpszCommand, [MarshalAs(UnmanagedType.LPStr)] StringBuilder lpszReturnString, int cchReturn, IntPtr hwndCallback);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/aa363858(v=vs.85).aspx
        [DllImport("kernel32")]
        private static extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/aa365747(v=vs.85).aspx
        [DllImport("kernel32")]
        private static extern bool WriteFile(
            IntPtr hFile,
            byte[] lpBuffer,
            uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten,
            IntPtr lpOverlapped);

        #endregion 
        private static string RootPath = AppContext.BaseDirectory;

        public static void ShutDown()
        {
            Process.Start("shutdown", "/s /t 0");
        }
        public static void Restart()
        {
            Process.Start("shutdown", "/r /t 0");
        }
        public static void Sleep()
        {
            SetSuspendState(false, true, true);
        }
        public static async void TakeScreenShot(string Window)
        {
            File.WriteAllText("Window.txt", $"{Window}");
            Process.Start("FluxGuard-GUI", "FluxGuard-GUI.exe");
        }
        public static string GetCpuAndRamUsage()
        {
            var ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            var availableRam = ramCounter.NextValue() + "MB";

            var cpuCounter = new PerformanceCounter
            {
                CategoryName = "Processor",
                CounterName = "% Processor Time",
                InstanceName = "_Total"
            };
            cpuCounter.NextValue();
            Thread.Sleep(1000);
            var cpuUsage = cpuCounter.NextValue() + "%";

            var result = $"مصرف پردازنده : {cpuUsage} , مصرف رم : {availableRam}";
            return result;
        }

        //public static List<string> GetAllWindowHandleNames()
        //{
        //    List<string> windowHandleNames = new();

        //    // دریافت لیست تمام فرآیندها
        //    foreach (Process process in Process.GetProcesses())
        //    {
        //        try
        //        {
        //            // به‌روزرسانی اطلاعات فرآیند
        //            process.Refresh();

        //            // بررسی وجود پنجره اصلی و عنوان آن
        //            if (process.MainWindowHandle != IntPtr.Zero &&
        //                !string.IsNullOrWhiteSpace(process.MainWindowTitle))
        //            {
        //                string processName = process.ProcessName;
        //                string windowTitle = process.MainWindowTitle;

        //                // ترکیب نام فرآیند و عنوان پنجره
        //                string combinedInfo = $"{processName}-{windowTitle}";

        //                // اگر طول ترکیب بیشتر از 40 کاراکتر است، کوتاه کردن آن
        //                if (combinedInfo.Length > 30)
        //                {
        //                    combinedInfo = combinedInfo.Substring(0, 27) + "...";
        //                }

        //                // افزودن به لیست
        //                windowHandleNames.Add(combinedInfo);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            // اگر مشکلی در دسترسی به فرآیند ایجاد شد، صرف نظر می‌کنیم
        //            Console.WriteLine($"Error accessing process {process.ProcessName}: {ex.Message}");
        //        }
        //    }

        //    return windowHandleNames;
        //}
        public static List<string> GetAllWindowHandleNames()
        {
            List<string> windowHandleNames = new();
            foreach (Process window in Process.GetProcesses())
            {
                window.Refresh();
                if (window.MainWindowHandle != IntPtr.Zero && !string.IsNullOrEmpty(window.MainWindowTitle))
                    windowHandleNames.Add(window.ProcessName);
            }
            return windowHandleNames;
        }
        public static void CloseApp(string NameWindow)
        {
            Process me = Process.GetCurrentProcess();

            try
            {
                foreach (Process p in Process.GetProcesses())
                {
                    if (p.Id != me.Id && p.ProcessName == NameWindow)
                        p.Kill();
                }
            }
            catch
            {
                NameWindow = ("");
            }
        }
        public static string GetUpTime()
        {
            using var upTime = new PerformanceCounter("System", "System Up Time");
            upTime.NextValue();
            string DT = TimeSpan.FromSeconds(upTime.NextValue()).ToString();
            int DTIndex = DT.LastIndexOf(':');
            return DT.Substring(0, DTIndex);
        }
        public static string GetActiveWindowTitle()
        {
            string CurrentActiveWindowTitle;
            try
            {
                IntPtr hwnd = GetForegroundWindow();
                GetWindowThreadProcessId(hwnd, out uint pid);
                Process p = Process.GetProcessById((int)pid);
                string title = p.MainWindowTitle;
                if (string.IsNullOrWhiteSpace(title))
                    title = p.ProcessName;
                CurrentActiveWindowTitle = title;
                return title;
            }
            catch (Exception)
            {
                return "Unknown";
            }
        }
    }
}
