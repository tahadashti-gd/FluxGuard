using FluxGuard.Core.Models;
using FluxGuard.Core.Utilities;
using Serilog;
using Spectre.Console;

public static class LogService
{
     //public static ILog Log = LogModel.MainLog;

    static string userName ="";
    static long chatId;
    private static void LogBase(string level, string prefix, string message, params object[] args)
    {
        switch (level.ToLower())
        {
            case "info":
                Log.Information($"[{prefix}] {{Message}}", args.Length > 0 ? args : new[] { message });
                break;
            case "warning":
                Log.Warning($"[{prefix}] {{Message}}", args.Length > 0 ? args : new[] { message });
                AnsiConsole.MarkupLine($"[yellow] '{prefix}' {message}[/]");
                break;
            case "error":
                Log.Error($"[{prefix}] {{Message}}", args.Length > 0 ? args : new[] { message });
                AnsiConsole.MarkupLine($"[red] '{prefix}' {message}[/]");
                break;
            default:
                Log.Information($"[{prefix}] {{Message}}", args.Length > 0 ? args : new[] { message });
                break;
        }
    }

    public static void LogUserCommand(string command)
    {
        LogBase("info", "Command", "[{Command}] from {UserName}:{UserId}", command, userName, chatId);
    }

    public static void LogSecurity(string message)
    {
        LogBase("warning", "Security", message);
    }

    public static void LogInformation(string message)
    {
        LogBase("info", "Information", message);
    }

    public static void LogWarning(string context, string message)
    {
        LogBase("warning", context, message);
    }

    public static void LogError(Exception ex, string context)
    {
        LogBase("error", context, ex.Message);
    }

    public static void LogServiceActivity(string message)
    {
        LogBase("info", "Service", message);
    }

    public static void LogUI(string message)
    {
        LogBase("info", "UI Manager", message);
    }

    // متد جدید برای لاگ پاسخ بات
    public static void LogBotResponse(string response, long chatId)
    {
        LogBase("info", "BotResponse", "Response to {ChatId}: {Response}", chatId, response);
    }
}