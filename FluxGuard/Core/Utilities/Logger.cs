using System.Globalization;
using Serilog;


namespace FluxGuard.Core.Utilities
{
    internal class Logger
    {
        // Initialize the logger with a file sink
        public static void Initialize()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/fluxguard.log", rollingInterval: RollingInterval.Day, shared: true)
                //.WriteTo.SQLite("logs/fluxguard.db", tableName: "Logs") // Optional SQLite sink
                .CreateLogger();
        }

        // Display recent logs from the last 'hours' specified
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

            // Read and filter logs based on timestamp
            foreach (var line in File.ReadLines(logFilePath))
            {
                string timestampStr = ExtractTimestamp(line);
                if (DateTime.TryParse(timestampStr, out DateTime logTime) && logTime >= cutoffTime)
                {
                    filteredLogs.Add(line);
                }
            }

            // Display logs if available
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

            // Reinitialize the logger after log reading
            Initialize();
        }

        // Extract timestamp from log line
        private static string ExtractTimestamp(string logLine)
        {
            string[] parts = logLine.Split(' ');
            return parts.Length > 2 ? $"{parts[0]} {parts[1]}" : "";
        }
    }
}
