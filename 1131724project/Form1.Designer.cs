namespace _1131724project
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
            pictureBox1 = new PictureBox();
            groupBox1 = new GroupBox();
            BtnOK = new Button();
            Rbtn3 = new RadioButton();
            Rbtn2 = new RadioButton();
            Rbtn1 = new RadioButton();
            PicShow = new PictureBox();
            PicLevel = new PictureBox();
            textBoxMessage = new TextBox();
            button1 = new Button();
            PicChance = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PicShow).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PicLevel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PicChance).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.Title2_removebg_preview;
            pictureBox1.Location = new Point(55, -1);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(310, 60);
            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(BtnOK);
            groupBox1.Controls.Add(Rbtn3);
            groupBox1.Controls.Add(Rbtn2);
            groupBox1.Controls.Add(Rbtn1);
            groupBox1.Location = new Point(26, 65);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(373, 50);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "請選擇一位女明星";
            // 
            // BtnOK
            // 
            BtnOK.BackColor = Color.RosyBrown;
            BtnOK.Font = new Font("Microsoft JhengHei UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 136);
            BtnOK.ForeColor = SystemColors.ButtonHighlight;
            BtnOK.Location = new Point(277, 8);
            BtnOK.Name = "BtnOK";
            BtnOK.Size = new Size(96, 42);
            BtnOK.TabIndex = 2;
            BtnOK.Text = "確定選擇";
            BtnOK.UseVisualStyleBackColor = false;
            BtnOK.Click += BtnOK_Click;
            // 
            // Rbtn3
            // 
            Rbtn3.AutoSize = true;
            Rbtn3.Font = new Font("Microsoft JhengHei UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 136);
            Rbtn3.Location = new Point(188, 22);
            Rbtn3.Name = "Rbtn3";
            Rbtn3.Size = new Size(83, 23);
            Rbtn3.TabIndex = 2;
            Rbtn3.TabStop = true;
            Rbtn3.Text = "Wheein";
            Rbtn3.UseVisualStyleBackColor = true;
            // 
            // Rbtn2
            // 
            Rbtn2.AutoSize = true;
            Rbtn2.Font = new Font("Microsoft JhengHei UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 136);
            Rbtn2.Location = new Point(93, 22);
            Rbtn2.Name = "Rbtn2";
            Rbtn2.Size = new Size(85, 23);
            Rbtn2.TabIndex = 1;
            Rbtn2.TabStop = true;
            Rbtn2.Text = "An yujin";
            Rbtn2.UseVisualStyleBackColor = true;
            // 
            // Rbtn1
            // 
            Rbtn1.AutoSize = true;
            Rbtn1.Font = new Font("Microsoft JhengHei UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 136);
            Rbtn1.Location = new Point(15, 22);
            Rbtn1.Name = "Rbtn1";
            Rbtn1.Size = new Size(72, 23);
            Rbtn1.TabIndex = 0;
            Rbtn1.TabStop = true;
            Rbtn1.Text = "Karina";
            Rbtn1.UseVisualStyleBackColor = true;
            // 
            // PicShow
            // 
            PicShow.Location = new Point(41, 121);
            PicShow.Name = "PicShow";
            PicShow.Size = new Size(256, 319);
            PicShow.TabIndex = 2;
            PicShow.TabStop = false;
            // 
            // PicLevel
            // 
            PicLevel.Image = Properties.Resources.Level0;
            PicLevel.Location = new Point(322, 121);
            PicLevel.Name = "PicLevel";
            PicLevel.Size = new Size(90, 217);
            PicLevel.SizeMode = PictureBoxSizeMode.AutoSize;
            PicLevel.TabIndex = 3;
            PicLevel.TabStop = false;
            // 
            // textBoxMessage
            // 
            textBoxMessage.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            textBoxMessage.Location = new Point(26, 441);
            textBoxMessage.Multiline = true;
            textBoxMessage.Name = "textBoxMessage";
            textBoxMessage.Size = new Size(308, 81);
            textBoxMessage.TabIndex = 4;
            textBoxMessage.Click += textBoxMessage_Click;
            textBoxMessage.Leave += textBoxMessage_Leave;
            // 
            // button1
            // 
            button1.BackColor = Color.DarkSeaGreen;
            button1.Font = new Font("Microsoft JhengHei UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 136);
            button1.ForeColor = SystemColors.ButtonHighlight;
            button1.Location = new Point(340, 442);
            button1.Name = "button1";
            button1.Size = new Size(77, 32);
            button1.TabIndex = 5;
            button1.Text = " 傳送➤";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // PicChance
            // 
            PicChance.Image = Properties.Resources.chance;
            PicChance.Location = new Point(326, 353);
            PicChance.Name = "PicChance";
            PicChance.Size = new Size(85, 85);
            PicChance.TabIndex = 6;
            PicChance.TabStop = false;
            PicChance.Click += PicChance_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Linen;
            ClientSize = new Size(424, 534);
            Controls.Add(PicChance);
            Controls.Add(button1);
            Controls.Add(textBoxMessage);
            Controls.Add(PicLevel);
            Controls.Add(PicShow);
            Controls.Add(groupBox1);
            Controls.Add(pictureBox1);
            Name = "Form1";
            Text = "s1131724";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)PicShow).EndInit();
            ((System.ComponentModel.ISupportInitialize)PicLevel).EndInit();
            ((System.ComponentModel.ISupportInitialize)PicChance).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private GroupBox groupBox1;
        private RadioButton Rbtn2;
        private RadioButton Rbtn1;
        private RadioButton Rbtn3;
        private Button BtnOK;
        private PictureBox PicShow;
        private PictureBox PicLevel;
        private TextBox textBoxMessage;
        private Button button1;
        private PictureBox PicChance;
    }
}
