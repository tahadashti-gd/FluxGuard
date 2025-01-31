using System.Diagnostics;
using System.Runtime.InteropServices;
namespace ScreenshotDemo;

public class ScreenshotHelper
{
    public static Image? GetBitmapScreenshot(string processName)
    {
        Image? img = null;

        Thread t = new(() =>
        {
            IntPtr handle = GetWindowHandle(processName);
            
            //Check if window is minimized and show it if needed
            if (User32.IsIconic(handle))
                User32.ShowWindowAsync(handle, User32.SHOWNORMAL);
            
            User32.SetForegroundWindow(handle);

            //ALT + PRINT SCREEN gets screenshot of focused window
            //See this article for key list
            SendKeys.SendWait("%({PRTSC})");
            Thread.Sleep(1000);

            //The GetImage function in WPF gets a bitmapsource image
            //This could be replaced with the Winforms getimage since that returns an image
            img = Clipboard.GetImage();
        });

        //Run your code from a thread that joins the STA Thread
        //If this is not done, clipboard functions won't work
        t.SetApartmentState(ApartmentState.STA);
        t.Start();
        t.Join();

        return img;

    }

    public static List<string> GetAllWindowHandleNames()
    {
        List<string> windowHandleNames = new();
        foreach (Process window in Process.GetProcesses())
        {
            window.Refresh();
            if (window.MainWindowHandle != IntPtr.Zero && !string.IsNullOrEmpty(window.MainWindowTitle))
                windowHandleNames.Add(window.ProcessName);
        }
        return windowHandleNames;
    }

    private static IntPtr GetWindowHandle(string name)
    {
        var process = Process.GetProcessesByName(name).FirstOrDefault();
        if (process != null && process.MainWindowHandle != IntPtr.Zero)
            return process.MainWindowHandle;

        return IntPtr.Zero;
    }

    //Functions utillizing the user32.dll 
    //Documentation on user32.dll - http://www.pinvoke.net/index.aspx
    private class User32
    {
        public const int SHOWNORMAL = 1;
        public const int SHOWMINIMIZED = 2;
        public const int SHOWMAXIMIZED = 3;

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool CloseClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        public static extern bool EmptyClipboard();

        [DllImport("user32.dll")]
        public static extern IntPtr GetOpenClipboardWindow();

    }

}
