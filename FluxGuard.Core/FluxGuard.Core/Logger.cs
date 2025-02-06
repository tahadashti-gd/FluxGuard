using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluxGuard.Core
{
    public class Logger
    {
        public static void Initialize()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/fluxguard.log", rollingInterval: RollingInterval.Day,shared:true)
                //.WriteTo.SQLite("logs/fluxguard.db", tableName: "Logs") 
                .CreateLogger();
        }

        public static void ShowRecentLogs(int hours)
        {
            var culture = new CultureInfo("en-US");
            string logFileName = $"fluxguard{DateTime.Now.ToString("yyyyMMdd", culture)}.log";
            string logFilePath = Path.Combine("logs", logFileName);

            if (!File.Exists(logFilePath))
            {
                Console.WriteLine("❌ Log file not found!");
                return;
            }

            Log.CloseAndFlush();
            DateTime cutoffTime = DateTime.Now.AddHours(-hours);
            var filteredLogs = new List<string>();

            foreach (var line in File.ReadLines(logFilePath))
            {
                string timestampStr = ExtractTimestamp(line);
                if (DateTime.TryParse(timestampStr, out DateTime logTime) && logTime >= cutoffTime)
                {
                    filteredLogs.Add(line);
                }
            }

            if (filteredLogs.Count == 0)
            {
                Console.WriteLine($"❌ No logs found in the last {hours} hours.");
                return;
            }

            Console.WriteLine($"📜 Logs from the last {hours} hours:");
            foreach (var log in filteredLogs)
            {
                Console.WriteLine(log);
            }
            Initialize();
        }

        private static string ExtractTimestamp(string logLine)
        {
            string[] parts = logLine.Split(' ');
            return parts.Length > 2 ? $"{parts[0]} {parts[1]}" : "";
        }

    }
    public static class LoggerService
    {
        public static void LogCommand(long userId,string userName, string command)
        {
            Log.Information("Command received: {Command} from {UserName}:{UserId}", command,userName, userId);
        }

        public static void LogSecurity(string message)
        {
            Log.Warning("Security alert: {Message}", message);
        }

        public static void LogError(Exception ex, string context)
        {
            Log.Error("❌ Exception in {Context}: {Message}", context, ex.Message);
        }
        public static void LogService(string message)
        {
            Log.Information("[Service] {Message}", message);
        }

        public static void LogUI(string message)
        {
            Log.Information("[UI Manager] {Message}", message);
        }
    }


}
