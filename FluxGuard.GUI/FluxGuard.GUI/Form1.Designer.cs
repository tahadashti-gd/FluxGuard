namespace FluxGuard.GUI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            notifyIcon = new NotifyIcon(components);
            statusStrip1 = new StatusStrip();
            StatusStrip = new ToolStripStatusLabel();
            Logo = new PictureBox();
            ConfigPanel = new GroupBox();
            LangSelect = new ComboBox();
            button3 = new Button();
            LangLbl = new Label();
            AutoStart = new CheckBox();
            ChatIdbox = new TextBox();
            chatIdLbl = new Label();
            tokenBox = new TextBox();
            tokenLbl = new Label();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Logo).BeginInit();
            ConfigPanel.SuspendLayout();
            SuspendLayout();
            // 
            // notifyIcon
            // 
            notifyIcon.Icon = (Icon)resources.GetObject("notifyIcon.Icon");
            notifyIcon.Text = "notifyIcon1";
            notifyIcon.Visible = true;
            notifyIcon.MouseDoubleClick += notifyIcon1_MouseDoubleClick;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { StatusStrip });
            statusStrip1.Location = new Point(0, 361);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(2, 0, 21, 0);
            statusStrip1.Size = new Size(775, 26);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // StatusStrip
            // 
            StatusStrip.Name = "StatusStrip";
            StatusStrip.Size = new Size(49, 20);
            StatusStrip.Text = "Status";
            // 
            // Logo
            // 
            Logo.Anchor = AnchorStyles.Right;
            Logo.Image = Properties.Resources.logo;
            Logo.Location = new Point(-2, -3);
            Logo.Margin = new Padding(0);
            Logo.Name = "Logo";
            Logo.Size = new Size(392, 107);
            Logo.SizeMode = PictureBoxSizeMode.CenterImage;
            Logo.TabIndex = 2;
            Logo.TabStop = false;
            // 
            // ConfigPanel
            // 
            ConfigPanel.Controls.Add(LangSelect);
            ConfigPanel.Controls.Add(button3);
            ConfigPanel.Controls.Add(LangLbl);
            ConfigPanel.Controls.Add(AutoStart);
            ConfigPanel.Controls.Add(ChatIdbox);
            ConfigPanel.Controls.Add(chatIdLbl);
            ConfigPanel.Controls.Add(tokenBox);
            ConfigPanel.Controls.Add(tokenLbl);
            ConfigPanel.Font = new Font("Sahel Black", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ConfigPanel.Location = new Point(9, 118);
            ConfigPanel.Name = "ConfigPanel";
            ConfigPanel.Size = new Size(756, 240);
            ConfigPanel.TabIndex = 4;
            ConfigPanel.TabStop = false;
            ConfigPanel.Text = "Config and Settings";
            // 
            // LangSelect
            // 
            LangSelect.AutoCompleteCustomSource.AddRange(new string[] { "Fa", "En" });
            LangSelect.FormattingEnabled = true;
            LangSelect.Items.AddRange(new object[] { "Fa", "En" });
            LangSelect.Location = new Point(162, 173);
            LangSelect.Name = "LangSelect";
            LangSelect.Size = new Size(189, 43);
            LangSelect.TabIndex = 6;
            // 
            // button3
            // 
            button3.Font = new Font("Sahel", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button3.Location = new Point(372, 172);
            button3.Name = "button3";
            button3.Size = new Size(191, 44);
            button3.TabIndex = 8;
            button3.Text = "Show logs";
            button3.UseVisualStyleBackColor = true;
            // 
            // LangLbl
            // 
            LangLbl.AutoSize = true;
            LangLbl.Font = new Font("Sahel", 11F, FontStyle.Bold);
            LangLbl.Location = new Point(6, 177);
            LangLbl.Name = "LangLbl";
            LangLbl.Size = new Size(150, 33);
            LangLbl.TabIndex = 6;
            LangLbl.Text = "Bot Language:";
            // 
            // AutoStart
            // 
            AutoStart.AutoSize = true;
            AutoStart.Font = new Font("Sahel", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            AutoStart.Location = new Point(590, 174);
            AutoStart.Name = "AutoStart";
            AutoStart.Size = new Size(140, 39);
            AutoStart.TabIndex = 4;
            AutoStart.Text = "Auto Start";
            AutoStart.UseVisualStyleBackColor = true;
            AutoStart.CheckedChanged += AutoStart_CheckedChanged;
            // 
            // ChatIdbox
            // 
            ChatIdbox.Location = new Point(145, 105);
            ChatIdbox.Name = "ChatIdbox";
            ChatIdbox.Size = new Size(588, 42);
            ChatIdbox.TabIndex = 3;
            ChatIdbox.KeyPress += textBox2_KeyPress;
            // 
            // chatIdLbl
            // 
            chatIdLbl.AutoSize = true;
            chatIdLbl.Font = new Font("Sahel", 11F, FontStyle.Bold);
            chatIdLbl.Location = new Point(6, 112);
            chatIdLbl.Name = "chatIdLbl";
            chatIdLbl.Size = new Size(133, 33);
            chatIdLbl.TabIndex = 2;
            chatIdLbl.Text = "User ChatID:";
            // 
            // tokenBox
            // 
            tokenBox.Location = new Point(229, 42);
            tokenBox.Name = "tokenBox";
            tokenBox.Size = new Size(504, 42);
            tokenBox.TabIndex = 1;
            // 
            // tokenLbl
            // 
            tokenLbl.AutoSize = true;
            tokenLbl.Font = new Font("Sahel", 11F, FontStyle.Bold);
            tokenLbl.Location = new Point(6, 49);
            tokenLbl.Name = "tokenLbl";
            tokenLbl.Size = new Size(207, 33);
            tokenLbl.TabIndex = 0;
            tokenLbl.Text = "Telegram bot token:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(12F, 35F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(237, 246, 251);
            ClientSize = new Size(775, 387);
            Controls.Add(ConfigPanel);
            Controls.Add(statusStrip1);
            Controls.Add(Logo);
            Font = new Font("Sahel", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 7, 4, 7);
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FluxGuard";
            Load += Form1_Load;
            Shown += Form1_Shown_1;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)Logo).EndInit();
            ConfigPanel.ResumeLayout(false);
            ConfigPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private NotifyIcon notifyIcon;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel StatusStrip;
        private PictureBox Logo;
        private GroupBox ConfigPanel;
        private TextBox ChatIdbox;
        private Label chatIdLbl;
        private TextBox tokenBox;
        private Label tokenLbl;
        private CheckBox AutoStart;
        private Label LangLbl;
        private Button button3;
        private ComboBox LangSelect;
    }
}
