using ScreenshotDemo;
using System.Drawing.Imaging;


namespace FluxGuard.GUI
{
    public partial class Form1 : Form
    {
        public static string RootPath = AppContext.BaseDirectory;
        public static string userChatId;
        public static string  telegramBotToken;

        public Form1()
        {
            InitializeComponent();
            //this.Hide();
            notifyIcon.Visible = true;
        }

        private void Form1_Load(object sender, EventArgs e)
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

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
