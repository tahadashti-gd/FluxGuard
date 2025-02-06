using System.Diagnostics;
using System.Runtime.InteropServices;
using FluxGuard.Core;
using Serilog;
using FluxGuard.GUI;

class Program
{
    static void Main(string[] args)
    {
        Logger.Initialize();
        MainCore botCore = new MainCore();
        try
        {
            botCore.StartBot();
            Process.Start("FluxGuard-GUI", "FluxGuard-GUI.exe");
            Log.Information("FluxGuard started at {Time}", DateTime.UtcNow);
        }
        catch (Exception ex)
        {

        }
        
        Log.Information("FluxGuard.GUI started successfully.");
        CLI.StartCLI();

    }
}
