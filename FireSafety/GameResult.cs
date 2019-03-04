using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FireSafety
{
    public partial class GameResult : Form
    {
        private bool isDragging = false;
        private Point oldPos;

        public GameResult()
        {
            InitializeComponent();
            GameTime.Text = utils.stats.GameTime.ToString(@"mm\:ss"); ;
            WrongAnswers.Text = utils.stats.WrongAnswers.ToString();
            TestTime.Text = utils.stats.TestTime.ToString(@"mm\:ss"); ;
            RightAnswer.Text = utils.stats.TestStats.ToString();
        }

        private void GameResult_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void GameResult_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void Panel2_MouseDown(object sender, MouseEventArgs e)
        {
            this.isDragging = true;
            this.oldPos = new Point();
            this.oldPos.X = e.X;
            this.oldPos.Y = e.Y;
        }

        private void Panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.isDragging)
            {
                Point tmp = new Point(this.Location.X, this.Location.Y);
                tmp.X += e.X - this.oldPos.X;
                tmp.Y += e.Y - this.oldPos.Y;
                this.Location = tmp;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            this.isDragging = false;
        }
    }
}
