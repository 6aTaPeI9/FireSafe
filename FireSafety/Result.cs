using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FireSafety
{
    public partial class Result : UserControl
    {
        public Result()
        {
            InitializeComponent();
            GameTime.Text = utils.stats.GameTime.ToString(@"mm\:ss"); ;
            WrongAnswers.Text = utils.stats.WrongAnswers.ToString();
            TestTime.Text = utils.stats.TestTime.ToString(@"mm\:ss"); ;
            RightAnswer.Text = utils.stats.TestStats.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
}
