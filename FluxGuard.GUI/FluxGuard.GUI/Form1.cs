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
            this.Hide();
            notifyIcon1.Visible = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void Form1_Shown(object sender, EventArgs e)
        {

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
                bitmap.Dispose();

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
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
        }

        private void Form1_Shown_1(object sender, EventArgs e)
        {
            Hide();
        }

        public static void CloseGUI()
        {
            Application.Exit();
        }
    }
}
