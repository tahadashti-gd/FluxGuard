using FluxGuard.Core;

class Program
{
    static void Main(string[] args)
    {
        MainCore botCore = new MainCore();
        botCore.StartBot();
        Console.WriteLine("Bot is running... Press any key to exit.");
        Console.ReadKey();
    }
}
