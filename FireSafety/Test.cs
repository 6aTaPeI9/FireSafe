using System.Windows.Forms;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace FireSafety
{
    public partial class Test : UserControl
    {

        LinkedQuestList _qs;

        private bool isDragging = false;
        private Point oldPos;
        private DateTime time_start;

        public Test()
        {
            InitializeComponent();
            FillQuest();
            FillflpIconQuestion();
            AddtlpMainRadio();
            //serialize_test();
            time_start = DateTime.Now;
            timer1.Start();
        }


        //Добавляем на нашу TableLayout радиобатоны
        private void AddtlpMainRadio()
        {
            Quest qs = _qs.GetCur();
            string[] quests = qs.Data.Split('#');

            QuestionLabel.Text = qs.Question;

            for (int i = 0; i <= quests.Length - 1; i++)
            {
                if (string.IsNullOrEmpty(quests[i]))
                    break;
                RadioButton rb = new RadioButton();
                if ((i == _qs.GetSelectedItem() - 1) && (_qs.GetSelectedItem() != 0))
                    rb.Checked = true;
                rb.CheckedChanged += new EventHandler(SetSelectedIndex);
                rb.Text = quests[i];
                rb.Font = new Font("Century Gothic", 12);
                rb.AutoSize = false;
                Size len = TextRenderer.MeasureText(quests[i], rb.Font);
                int height = (len.Width / 650 * 23) + 23;
                if (height == 0)
                    height = 17;
                rb.Size = new Size(700, height);
                tlpMainRadio.Controls.Add(rb, 0, i);
                tlpMainRadio.RowCount++;
            }
        }


        //Метод запоминает индекс радиобатона который выбрал пользователь 
        //и окрашивает кнопку с номером вопроса в зеленый цвет
        private void SetSelectedIndex(object sender, System.EventArgs e)
        {
            int i = 1;
            foreach (RadioButton myradbut in tlpMainRadio.Controls)
            {
                if (myradbut.Checked == true)
                {
                    _qs.SetSelectedItem(i);
                }
                i++;
            }

            foreach (QuestPanel btn in flpIconQuestion.Controls)
            {
                if (btn._index == _qs.GetCurIndex() + 1)
                {
                    btn.button1.BackColor = Color.FromArgb(178, 8, 55);
                }
            }
        }

        //Метод заполняет двусвязный список Quests.
        private void FillQuest()
        {
            //LinkedQuestList qs = new LinkedQuestList();
            //using (StreamReader rdr = new StreamReader(@"questions.txt"))
            //{
            //    string textfromfile = rdr.ReadToEnd();
            //    foreach (string question in Regex.Split(textfromfile, @"-------------------------------\r\n"))
            //    {
            //        List<string> sub_question = Regex.Split(question, "\r\n").Where(x => x != String.Empty).ToList();
            //        int right_answer = 1;
            //        for (int i = 1; i <= sub_question.Count() - 1; i++)
            //        {
            //            if (char.IsUpper(sub_question[i][0]))
            //            {
            //                right_answer = i;
            //            }

            //            sub_question[i] = Regex.Split(sub_question[i], @"%%")[1];
            //        }
            //        string Data = string.Join("#", sub_question.GetRange(1, sub_question.Count - 1));

            //        qs.Add(Data, sub_question[0], right_answer);
            //    }
            //}
            //_qs = qs;

            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream fs = new FileStream("Demon.dat", FileMode.OpenOrCreate))
            {
                _qs = (LinkedQuestList)formatter.Deserialize(fs);
            }
        }

        //Заполняем правую панель flowlayout пользовательскими контролами QuestPanel
        private void FillflpIconQuestion()
        {
            QuestPanel qp;
            for (int i = 0; i <= _qs.count - 1; i++)
            {
                if (i == 0)
                {
                    qp = new QuestPanel(i + 1, _qs);
                }
                else
                {
                    qp = new QuestPanel(i + 1, _qs);

                }
                flpIconQuestion.Controls.Add(qp);
            }
            _qs.SetCurByIndex(0);
        }

        //Метод очищает TableLayout от всех radiobutton и заполняет контейнер новыми переключателями
        private void clear_table_layout()
        {
            List<Control> listControls = tlpMainRadio.Controls.Cast<Control>().ToList();

            foreach (Control control in listControls)
            {
                tlpMainRadio.Controls.Remove(control);
                control.Dispose();
            }

            AddtlpMainRadio();
            PointsLabel.Text = (_qs.GetCurIndex() + 1).ToString();
        }

        //Событие при нажатии на один из контролов с номером вопроса
        private void draw_cur_quest(object sender, System.EventArgs e)
        {
            clear_table_layout();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            _qs.GetPrev();
            clear_table_layout();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            foreach (RadioButton myradbut in tlpMainRadio.Controls)
            {
                if (myradbut.Checked == true)
                {
                    if(_qs.IsLast() == true)
                    {
                        utils.stats.TestStats = _qs.GetHappyIndex();
                        utils.stats.TestTime = DateTime.Now - time_start;
                        this.Parent.Controls.Add(new Result());
                        this.Parent.Controls.Remove(this);
                    }
                    _qs.GetNext();
                    clear_table_layout();
                    return;
                }
            }
            MessageBox.Show("Выберите вариант ответа.");

        }


        private void serialize_test()
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream fs = new FileStream("Demon.dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, _qs);
            }
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeSpan delta = DateTime.Now - time_start;
            TimeLabe.Text = delta.ToString(@"mm\:ss");
        }
    }
}
