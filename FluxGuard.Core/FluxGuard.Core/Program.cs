using System.Diagnostics;
using System.Runtime.InteropServices;
using FluxGuard.Core;
using FluxGuard.GUI;

class Program
{
    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    const int SW_HIDE = 0;
    const int SW_SHOW = 5;
    static void Main(string[] args)
    {
        var handle = GetConsoleWindow();
        AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        MainCore botCore = new MainCore();
        botCore.StartBot();
        Process.Start("FluxGuard-GUI", "FluxGuard-GUI.exe");
        Console.WriteLine("Bot is running...");
        Console.WriteLine("pres any key to hide console...");
        Console.ReadKey();
        ShowWindow(handle, 0);
        Console.ReadKey();
    }
    static void OnProcessExit(object sender, EventArgs e)
    {
        Form1.CloseGUI();
        // اینجا هر کاری که لازم داری انجام بده، مثل ذخیره اطلاعات
    }
}
