﻿using System;
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
    public partial class QuestPanel : UserControl
    {
        LinkedQuestList _qs;
        public int _index;

        public QuestPanel(int index, LinkedQuestList qs)
        {
            InitializeComponent();
            _index = index;
            _qs = qs;
            this.button1.Text = index.ToString();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //_qs.SetCurByIndex(_index - 1);
        }
    }
}
