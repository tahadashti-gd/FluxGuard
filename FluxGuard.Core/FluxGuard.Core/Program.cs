using System.Diagnostics;
using FluxGuard.Core;      // Importing FluxGuard.Core for core functionalities
using FluxGuard.GUI;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            Process.Start(AppContext.BaseDirectory+"/FluxGuard.GUI.exe");

            // Initialize the logging system
            Logger.Initialize();

            // Initialize the data management system
            DataManager.Initialize();

            // Check if the application is set to start automatically
            if (DataManager.Setting.AutomaticStart)
            {
                // Start the bot if automatic start is enabled
                CLI.StartBot();
            }

            // Start the command-line interface for user interaction
            CLI.StartCLI();


        }
        catch (Exception ex)
        {
            // Log the error with details for debugging
            LoggerService.LogError(ex, "Initializing program");
        }
    }
    static void OnProcessExit(object sender, EventArgs e)
    {
        Form1.CloseGUI();
        LoggerService.LogInformation("sfds");
    }
}
