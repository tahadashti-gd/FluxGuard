using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;

namespace SysInfo
{
    class SystemInfo
    {
        public static StringBuilder FReport;
        public static void Main()
        {
            var report = new StringBuilder();
            report.AppendLine("===== گزارش مصرف منابع سیستم =====");

            AppendCPUUsage(report);
            AppendMemoryUsage(report);
            AppendDiskUsage(report);
            AppendNetworkUsage(report);
            AppendGPUUsage(report);

            report.AppendLine("======================");

            FReport = report;
        }

        static void AppendCPUUsage(StringBuilder report)
        {
            report.AppendLine("\n[اطلاعات پردازنده]");
            var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue();
            System.Threading.Thread.Sleep(1000); // برای به‌دست آوردن مقدار دقیق‌تر
            float cpuUsage = cpuCounter.NextValue();
            report.AppendLine($"درصد استفاده از پردازنده: {cpuUsage:F2}%");
        }

        static void AppendMemoryUsage(StringBuilder report)
        {
            report.AppendLine("\n[اطلاعات حافظه]");
            var totalMemory = new PerformanceCounter("Memory", "Committed Bytes").NextValue();
            var availableMemory = new PerformanceCounter("Memory", "Available Bytes").NextValue();
            float usedMemory = totalMemory - availableMemory;
            report.AppendLine($"کل حافظه: {totalMemory / (1024 * 1024 * 1024):F2} گیگابایت");
            report.AppendLine($"حافظه استفاده شده: {usedMemory / (1024 * 1024 * 1024):F2} گیگابایت");
        }

        static void AppendDiskUsage(StringBuilder report)
        {
            report.AppendLine("\n[اطلاعات دیسک]");
            var category = new PerformanceCounterCategory("LogicalDisk");
            var instanceNames = category.GetInstanceNames();

            foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady))
            {
                string instanceName = instanceNames.FirstOrDefault(name => name.StartsWith(drive.Name.Substring(0, 1)));
                if (!string.IsNullOrEmpty(instanceName))
                {
                    var diskUsage = new PerformanceCounter("LogicalDisk", "% Disk Time", instanceName);
                    diskUsage.NextValue();
                    System.Threading.Thread.Sleep(1000);
                    float usage = diskUsage.NextValue();
                    report.AppendLine($"درایو {drive.Name}: {usage:F2}% استفاده");
                }
                else
                {
                    report.AppendLine($"اینستنس مرتبط با درایو {drive.Name} پیدا نشد.");
                }
            }
        }

        static void AppendNetworkUsage(StringBuilder report)
        {
            report.AppendLine("\n[اطلاعات شبکه]");
            var category = new PerformanceCounterCategory("Network Interface");
            var instanceNames = category.GetInstanceNames();

            if (instanceNames.Length > 0)
            {
                string instanceName = instanceNames[0]; // اولین آداپتور شبکه
                var netSentCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", instanceName);
                var netReceivedCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", instanceName);

                System.Threading.Thread.Sleep(1000);
                float sent = netSentCounter.NextValue();
                float received = netReceivedCounter.NextValue();

                report.AppendLine($"نام آداپتور: {instanceName}");
                report.AppendLine($"ارسال داده: {sent / 1024:F2} کیلوبایت بر ثانیه");
                report.AppendLine($"دریافت داده: {received / 1024:F2} کیلوبایت بر ثانیه");
            }
            else
            {
                report.AppendLine("هیچ آداپتور شبکه فعالی پیدا نشد.");
            }
        }

        static void AppendGPUUsage(StringBuilder report)
        {
            report.AppendLine("\n[اطلاعات کارت گرافیک]");
            var searcher = new ManagementObjectSearcher("select * from Win32_VideoController");
            foreach (var item in searcher.Get())
            {
                report.AppendLine($"نام: {item["Name"]}");
                report.AppendLine($"رم: {Convert.ToInt64(item["AdapterRAM"]) / (1024 * 1024)} مگابایت");
            }
        }
    }
}

