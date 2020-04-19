using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace FireSafety
{
    public partial class Game : UserControl
    {

        private bool isDragging = false;
        private Point oldPos;
        int start_x;
        int start_y;
        Dictionary<string, Image> images;
        List<utils.GameImages> deserilize_game;
        List<utils.TargetImages> deserilize;
        int count_target_rows = 0;
        int count_target_pb = 0;
        int complited_rows = 0;
        List<Panel> targets_pb;
        System.Windows.Forms.Control parent_panel;
        int all_attemp = 0;
        int good_attemp = 0;
        string folderPath = Environment.CurrentDirectory + "\\GameImages";
        DateTime time_start;

        public Game()
        {
            InitializeComponent();
            Test gm = new Test();
            DeserializeMainPage();
            DeserializeGameImage();
            BuildMainPage();
            PrintImagesOnForm();
            time_start = DateTime.Now;
            timer1.Start();
        }

        private void DeserializeMainPage()
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream fs = new FileStream("targets.dat", FileMode.OpenOrCreate))
            {
                deserilize = (List<utils.TargetImages>)formatter.Deserialize(fs);
            }
        }

        private void DeserializeGameImage()
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream fs = new FileStream("AnswersImages.dat", FileMode.OpenOrCreate))
            {
                deserilize_game = (List<utils.GameImages>)formatter.Deserialize(fs);
            }
        }

        private void PrintImagesOnForm()
        {
            int x = 5;
            int y = 15;
            int def_size_x = 50;
            int def_size_y = 50;
            int separator_x = 10;
            int separator_y = 15;
            int i = 0;
            int i_dx = 0;
            images = new Dictionary<string, Image>();
            foreach (var game_images in deserilize_game)
            {
                if (i_dx == 3)
                {
                    i_dx = 0;
                    i++;
                }
                Panel pn = new Panel();
                pn.MouseEnter += Pn_MouseEnter;
                pn.MouseLeave += Pn_MouseLeave;
                PictureBox pb = new PictureBox();
                pb.Name = game_images.img_name;
                pb.Image = game_images.img;
                pb.Size = new Size(def_size_x, def_size_y);
                pn.Size = new Size(def_size_x + 10, def_size_y + 10);
                pb.Location = new Point(5, 5);
                pn.Location = new Point((x + def_size_y * i_dx) + separator_x * i_dx,
                                        (y + def_size_y * i) + separator_y * i);
                pb.SizeMode = PictureBoxSizeMode.StretchImage;
                pb.BorderStyle = BorderStyle.None;
                FireSafety.Move.LearnToMove(pb);
                pb.MouseUp += PictureBox1_MouseUp;
                pb.MouseDown += PictureBox1_MouseDown;
                pb.MouseEnter += Pb_MouseEnter;
                pb.MouseLeave += Pb_MouseLeave;
                pb.LocationChanged += Pb_LocationChanged;
                pn.Controls.Add(pb);
                MainPanel.Controls.Add(pn);
                i_dx++;
            }
        }

        private void Pb_LocationChanged(object sender, EventArgs e)
        {
            Control ctr = (Control)sender;
            int x = ctr.Location.X;
            int y = ctr.Location.Y;
            if (complited_rows < count_target_rows)
            {
                var row = deserilize[complited_rows];
                foreach (KeyValuePair<string, string> info in row.coord_blockers)
                {
                    if (Controls.ContainsKey(info.Key))
                    {
                        Panel finded_pb = (Panel)Controls[info.Key];
                        int x_t = Convert.ToInt32(info.Value.Split(':')[0]);
                        int y_t = Convert.ToInt32(info.Value.Split(':')[1]);

                        if (x > x_t && x < x_t + finded_pb.Width &&
                            y > y_t && y < y_t + finded_pb.Height)
                        {
                            finded_pb.BackColor = Color.FromArgb(178, 8, 55);
                        }
                        else
                        {
                            finded_pb.BackColor = Color.FromArgb(41, 39, 40);
                        }
                    }
                }
            }

        }

        private void Pb_MouseLeave(object sender, EventArgs e)
        {
            Control pb = (Control)sender;
            if (pb.Parent != this)
                pb.Parent.BackColor = Color.FromArgb(41, 39, 40);
        }

        private void Pb_MouseEnter(object sender, EventArgs e)
        {
            Control pb = (Control)sender;
            if (pb.Parent != this)
                pb.Parent.BackColor = Color.FromArgb(178, 8, 55);
        }

        private void Pn_MouseLeave(object sender, EventArgs e)
        {
            Panel pn = (Panel)sender;
            pn.BackColor = Color.FromArgb(41, 39, 40);
        }

        private void Pn_MouseEnter(object sender, EventArgs e)
        {
            Panel pn = (Panel)sender;
            pn.BackColor = Color.FromArgb(178, 8, 55);
        }

        private void BuildMainPage()
        {
            var row = deserilize[complited_rows];

            Image def_image = Image.FromFile(Application.StartupPath + "\\Design\\input.jpg");
            count_target_rows = deserilize.Count();
            count_target_pb = row.coord_blockers.Count();
            targets_pb = new List<Panel>();

            foreach (KeyValuePair<string, string> info in row.coord_blockers)
            {
                PictureBox pb = new PictureBox();
                Panel pn = new Panel();
                pn.BackColor = Color.FromArgb(41, 39, 40);
                pb.MouseEnter += Pb_MouseEnter;
                pb.MouseLeave += Pb_MouseLeave;
                pb.Image = def_image;
                pn.Name = info.Key;
                pn.Size = new Size(75, 75);
                pb.Size = new Size(60, 60);
                pb.SizeMode = PictureBoxSizeMode.StretchImage;
                int x = Convert.ToInt32(info.Value.Split(':')[0]);
                int y = Convert.ToInt32(info.Value.Split(':')[1]);
                pn.Location = new Point(x, y);
                pb.Location = new Point(7, 7);
                pn.Controls.Add(pb);
                this.Controls.Add(pn);
                pn.BringToFront();
                targets_pb.Add(pn);
            }

            MainPictureBox.Image = row.img;
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {

            Control ctr = (Control)sender;
            if (Controls.ContainsKey(ctr.Name.ToString() + "_Target"))
            {
                Panel finded_pb = (Panel)Controls[ctr.Name.ToString() + "_Target"];
                int x = finded_pb.Location.X;
                int y = finded_pb.Location.Y;
                int _x = ctr.Location.X;
                int _y = ctr.Location.Y;
                if (_x > x && _x < x + finded_pb.Width &&
                    _y > y && _y < y + finded_pb.Height)
                {
                    Controls.Remove(finded_pb);
                    targets_pb.Remove(finded_pb);
                    count_target_pb--;
                    good_attemp++;
                    if (count_target_pb == 0)
                    {
                        complited_rows++;
                        if (complited_rows < count_target_rows)
                        {
                            BuildMainPage();
                        }
                        else
                        {
                            utils.stats.GameTime = DateTime.Now - time_start;
                            utils.stats.WrongAnswers = all_attemp;
                            utils.stats.RightAnswers = good_attemp;
                            Test gm = new Test();
                            this.Parent.Controls.Add(gm);
                            this.Parent.Controls.Remove(this);
                        }
                    }
                }
                else
                {
                    all_attemp++;
                }
            }
            
            PointsLabel.Text = all_attemp.ToString();
            ctr.Parent = parent_panel;
            ctr.Location = new Point(start_x, start_y);
        }

        private void serialize_test()
        {
            BinaryFormatter formatter = new BinaryFormatter();

            string searchPattern = "*.png";
            string[] FilesName = Directory.GetFiles(folderPath, searchPattern);
            List<utils.GameImages> gimg = new List<utils.GameImages>();

            foreach (string file_name in FilesName)
            {
                Image img = Image.FromFile(file_name);
                utils.GameImages gi = new utils.GameImages(file_name.Split('.')[0].Split('\\').Last(), img);
                gimg.Add(gi);
            }

            using (FileStream fs = new FileStream("Test.dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, gimg);
            }
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            Control ctr = (Control)sender;
            start_x = ctr.Location.X;
            start_y = ctr.Location.Y;
            parent_panel = ctr.Parent;
            ctr.Parent = this;
            ctr.BringToFront();
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
            TimeLabe.Text = delta.ToString().Substring(0,8);
        }
    }
}
