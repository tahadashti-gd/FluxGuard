using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;
using System.Management;
using FluxGuard.Core.Services;

namespace FluxGuard.Core.FLuxModules
{
    internal class StatusModule
    {
        public static string GetUpTime()
        {
            using var upTime = new PerformanceCounter("System", "System Up Time");
            upTime.NextValue();
            string DT = TimeSpan.FromSeconds(upTime.NextValue()).ToString();
            int DTIndex = DT.LastIndexOf(':');
            LogService.LogServiceActivity("GetUpTime");

            return DT.Substring(0, DTIndex);
        }
        #region Resource report
        public static string GetSystemUsageReport()
        {
            StringBuilder report = new StringBuilder();
            report.AppendLine(LanguageService.Translate("answers", "resource", "report"));
            report.AppendLine(LanguageService.Translate("answers", "resource", "cpu") + GetCpuUsage() + "%");
            report.AppendLine(LanguageService.Translate("answers", "resource", "ram") + GetRamUsage() + "%");
            report.AppendLine(LanguageService.Translate("answers", "resource", "gpu") + GetGpuUsage() + "%");
            report.AppendLine(LanguageService.Translate("answers", "resource", "net") + GetInternetSpeed() + "Mb/s");
            LogService.LogServiceActivity($"GetSystemUsageReport");

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
