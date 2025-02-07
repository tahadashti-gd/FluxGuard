using System.Diagnostics;
using FluxGuard.Core;
using Serilog;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            Logger.Initialize();
            CLI.StartCLI();
        }
        catch (Exception ex)
        {

        }
        
    }
}
