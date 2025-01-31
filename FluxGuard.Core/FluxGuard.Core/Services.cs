using System.Diagnostics;
using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using FluxGuard.GUI;
using Languages;

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

        #region Power
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
        #endregion

        public static async void TakeScreenShot(string Window)
        {
            Form1.Screenshot(Window);
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


        #region Resource report
        public static string GetSystemUsageReport()
        {
            StringBuilder report = new StringBuilder();

            report.AppendLine(Lang.Translate("answers", "resource", "report"));
            report.AppendLine(Lang.Translate("answers", "resource", "cpu") + GetCpuUsage() + "%");
            report.AppendLine(Lang.Translate("answers", "resource", "ram") + GetRamUsage() + "%");
            report.AppendLine(Lang.Translate("answers", "resource", "gpu") + GetGpuUsage() + "%");
            report.AppendLine(Lang.Translate("answers", "resource", "net") + GetInternetSpeed() + "Mb/s");

            return report.ToString();
        }
        private static float GetCpuUsage()
        {
            var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue();
            Thread.Sleep(500); // صبر برای نمونه‌گیری
            return (float)Math.Round(cpuCounter.NextValue(), 2);
        }
        private static float GetRamUsage()
        {
            var ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
            return (float)Math.Round(ramCounter.NextValue(), 2);
        }
        private static float GetGpuUsage()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("select * from Win32_VideoController"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        if (obj["AdapterRAM"] != null)
                        {
                            return (float)Math.Round(Convert.ToDouble(obj["AdapterRAM"]) / 1073741824, 2); // تبدیل به گیگابایت
                        }
                    }
                }
            }
            catch (Exception) { }
            return 0; // اگر اطلاعات GPU پیدا نشد
        }
        private static double GetInternetSpeed()
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            long totalBytesReceived1 = 0;

            foreach (NetworkInterface ni in interfaces)
            {
                if (ni.OperationalStatus == OperationalStatus.Up)
                {
                    totalBytesReceived1 += ni.GetIPv4Statistics().BytesReceived;
                }
            }

            Thread.Sleep(1000); // یک ثانیه صبر کن تا مقدار جدید دریافت شود

            long totalBytesReceived2 = 0;
            foreach (NetworkInterface ni in interfaces)
            {
                if (ni.OperationalStatus == OperationalStatus.Up)
                {
                    totalBytesReceived2 += ni.GetIPv4Statistics().BytesReceived;
                }
            }

            // محاسبه میزان دانلود در یک ثانیه (بایت)
            long bytesReceivedPerSecond = totalBytesReceived2 - totalBytesReceived1;

            // تبدیل به MB/s (مگابایت بر ثانیه)
            double speedMBps = bytesReceivedPerSecond / (1024.0 * 1024.0);

            return Math.Round(speedMBps, 2);
        }
        #endregion

    }
}
