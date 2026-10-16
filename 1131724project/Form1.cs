namespace _1131724project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string selectedIdol = "";
        string[] IdolQuotes = new string[]
        {
            "今天過的好嗎？我今天特別特別開心喔！\n我會把你的今天變得讓你滿意的！",
            "我最近在練習即將回歸的主打曲, 偷偷提前讓你聽, 你願意聽嗎?",
            "我想快點在演唱會上見到你, 每天都在努力準備中喔！",
            "做我的小狗吧...",
            "今晚有空嗎？要一起出去走走嗎？", 
            "你怎麼一直在看小卡?\n我明明就在你的眼前...", 
            "我們團內成員中你最喜歡的是誰?", 
            "好想跟你見面喔!但是這個月都在世巡..."
        };
        int i = 0, count = 0;
        Random rnd = new Random();    // rnd 為亂數物件

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] chanceSuccessImgs = {"chance_success1.png","chance_success2.png","chance_success3.png" };
            string chanceFailImg = "chance_fail.png";
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (Rbtn1.Checked)
            {
                selectedIdol = "Karina";
                PicShow.Image = Image.FromFile("result_Karina.jpg");
            }
            else if (Rbtn2.Checked)
            {
                selectedIdol = "An yujin";
                PicShow.Image = Image.FromFile("result_AnYujin.jpg");
            }
            else if (Rbtn3.Checked)
            {
                selectedIdol = "Wheein";
                PicShow.Image = Image.FromFile("result_Wheein.jpg");
            }
            // 按下確定按鈕後將所有選項的 Enabled 屬性改成 false
            Rbtn1.Enabled = false;
            Rbtn2.Enabled = false;
            Rbtn3.Enabled = false;
            BtnOK.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBoxMessage.Text))
            {
                count++;
                int index = rnd.Next(IdolQuotes.Length);
                textBoxMessage.Text = selectedIdol + ": " + IdolQuotes[index];
                if (i == 3)
                {
                    MessageBox.Show("成功與女明星談上戀愛, \n這輩子算是值了！", "*** 達成成就 ***");
                    Application.Exit();
                }
                else
                {
                    if (count > 0 && count % 3 == 0)
                    {
                        i++;
                        string levelImg = $"level{i}.png";
                        PicLevel.Image = Image.FromFile(levelImg);
                    }
                }
            }
            else
            {
                MessageBox.Show("請先輸入訊息再按傳送");
            }
        }
        bool isFirstClick = true;
        private void textBoxMessage_Click(object sender, EventArgs e)
        {
            if (isFirstClick)
            {
                textBoxMessage.SelectAll();
                isFirstClick = false;
            }
        }

        private void textBoxMessage_Leave(object sender, EventArgs e)
        {
            isFirstClick = true;
        }

        private void PicChance_Click(object sender, EventArgs e)
        {
            int chance = rnd.Next(1, 11);
            if(chance >= 1 && chance <= 3)
            {
                PicLevel.Image = Image.FromFile("Level3.png");
                PicChance.Image = Image.FromFile("success.jpg");
                MessageBox.Show($"一鍵速通!!!\n你生來就是要和{selectedIdol}的天選之子", "*** 達成成就 ***");
                Application.Exit();
            }
            else
            {
                PicLevel.Image = Image.FromFile("Level0.png");
                PicChance.Image = Image.FromFile("fail.jpg");
                i = 0;
                count = 0;
                MessageBox.Show($"你還真是賴蛤麻想吃天鵝肉!\n與{selectedIdol}的好感值歸零!", "*** 大失敗 ***");
            }
        }
    }
}
