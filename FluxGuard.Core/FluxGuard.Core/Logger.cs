using Serilog;
using System;
using System.Collections.Generic;
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
                .WriteTo.Console()
                .WriteTo.File("logs/fluxguard.log", rollingInterval: RollingInterval.Day)
                //.WriteTo.SQLite("logs/fluxguard.db", tableName: "Logs") 
                .CreateLogger();
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
