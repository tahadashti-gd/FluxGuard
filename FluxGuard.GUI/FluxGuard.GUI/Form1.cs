using ScreenshotDemo;
using System.Drawing.Imaging;
using System.Drawing;

namespace FluxGuard.GUI
{
    public partial class Form1 : Form
    {
        public static string RootPath = AppContext.BaseDirectory;
        string Window = "";
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ScreenshotHelper.GetAllWindowHandleNames();

            ////Window = File.ReadAllText("Window.txt");

            //if (Window == "Screen")
            //{
            //    var bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            //    using var graphics = Graphics.FromImage(bitmap);
            //    graphics.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);
            //    var fileName = "Screenshot.png";
            //    bitmap.Save(fileName, ImageFormat.Png);

            //}
            //else
            //{
            //    string selection = Window;
            //    if (string.IsNullOrEmpty(selection))
            //    {
            //        MessageBox.Show("خطا");
            //        return;
            //    }
            //    var img = ScreenshotHelper.GetBitmapScreenshot(selection);
            //    if (img == null)
            //    {
            //        MessageBox.Show("خطا");
            //        return;
            //    }
            //    img.Save($"{RootPath}Screenshot.png", ImageFormat.Png);
            //}
            //Thread.Sleep(1000);
            //this.Close();
        }

        public static void Screenshot(string screenName)
        {
            if (screenName == "Screen")
            {
                var bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                using var graphics = Graphics.FromImage(bitmap);
                graphics.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);
                var fileName = "Screenshot.png";
                bitmap.Save(fileName, ImageFormat.Png);

            }
            else
            {
                string selection = screenName;
                if (string.IsNullOrEmpty(selection))
                {
                    MessageBox.Show("خطا");
                    return;
                }
                var img = ScreenshotHelper.GetBitmapScreenshot(selection);
                if (img == null)
                {
                    MessageBox.Show("خطا");
                    return;
                }
                img.Save($"{RootPath}Screenshot.png", ImageFormat.Png);
            }
            Thread.Sleep(1000);
        }

        private void label1_Click(object sender, EventArgs e)
        {

            //label1.Text = MainCore.BotLang;
        }
    }
}
