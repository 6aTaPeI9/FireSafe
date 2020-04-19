using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FireSafety
{
    public partial class FareSafetyStart : UserControl
    {
        public FareSafetyStart()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Parent.Controls.Add(new Game());
            this.Parent.Controls.Remove(this);
        }
    }
}
