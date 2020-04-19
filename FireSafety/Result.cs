using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace FireSafety
{
    public partial class Result : UserControl
    {
        public Result()
        {
            InitializeComponent();
            int right_percentage = utils.stats.TestStats;
            GameTime.Text = utils.stats.GameTime.ToString().Substring(0, 8);
            WrongAnswers.Text = utils.stats.WrongAnswers.ToString();
            GoodAnswer.Text = utils.stats.RightAnswers.ToString();
            TestTime.Text = utils.stats.TestTime.ToString().Substring(0, 8);
            RightAnswer.Text = right_percentage.ToString() + " ("+(float)right_percentage/25*100+"%)";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Parent.Controls.Add(new FareSafetyStart());
            this.Parent.Controls.Remove(this);
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
