using FluxGuard.Core;      // Importing FluxGuard.Core for core functionalities
using Spectre.Console;     // Importing Spectre.Console for enhanced console output

class Program
{
    static void Main(string[] args)
    {
        try
        {
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
}
