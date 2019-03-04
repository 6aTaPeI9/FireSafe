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
        string folderPath = Environment.CurrentDirectory + "\\GameImages";
        DateTime time_start;

        public Game()
        {
            InitializeComponent();
            //serialize_list_images();
            //serialice_game_images();
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
                            Test gm = new Test();
                            this.Parent.Controls.Add(gm);
                            this.Parent.Controls.Remove(this);
                        }
                    }
                }
            }
            all_attemp++;
            PointsLabel.Text = all_attemp.ToString();
            ctr.Parent = parent_panel;
            ctr.Location = new Point(start_x, start_y);
        }

        private void serialize_list_images()
        {
            BinaryFormatter formatter = new BinaryFormatter();

            string searchPattern = "*.bmp";
            string[] FilesName = Directory.GetFiles(folderPath, searchPattern);
            List<utils.GameImages> gimg = new List<utils.GameImages>();

            foreach (string file_name in FilesName)
            {
                Image img = Image.FromFile(file_name);
                utils.GameImages gi = new utils.GameImages(file_name.Split('.')[0].Split('\\').Last(), img);
                gimg.Add(gi);
            }

            using (FileStream fs = new FileStream("AnswersImages.dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, gimg);
            }
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

        private void serialice_game_images()
        {
            string image_1 = Environment.CurrentDirectory + "\\Design\\Znaki-pozh-bezopasnosti.jpg";
            string image_2 = Environment.CurrentDirectory + "\\Design\\Без имени2.bmp";
            string image_3 = Environment.CurrentDirectory + "\\Design\\z12.jpg";
            string image_4 = Environment.CurrentDirectory + "\\Design\\Fire.bmp";
            Image img1 = Image.FromFile(image_1);
            Image img2 = Image.FromFile(image_2);
            Image img3 = Image.FromFile(image_3);
            Image img4 = Image.FromFile(image_4);

            List<utils.TargetImages> targets = new List<utils.TargetImages>();
            Dictionary<string, string> numbers = new Dictionary<string, string>
            {
                {"ExitLeft_Target", "246:83"},
                {"ExitRight_Target", "460:83"},
                {"Evacuation_Target", "247:173"},
                {"ForOpenDestroyHere_Target", "459:173"},

                {"Arrow_Target", "246:294"},
                {"Arrow45_Target", "460:294"},
                {"FireCock_Target", "675:294"},

                {"Stairs_Target", "246:383"},
                {"Extinguish_Target", "460:383"},
                {"Telephone_Target", "675:383"},

                {"PlaceWithFireInvertory_Target", "246:469"},
                {"WaterSource_Target", "460:469"},
                {"FireStand_Target", "675:469"},

                {"FireGidrant_Target", "246:554"},
                {"ButtonFireSettings_Target", "460:554"},
                {"Annunciator_Target", "675:554"}
            };

            Dictionary<string, string> numbers2 = new Dictionary<string, string>
            {
                {"WorkOnGlasses_Target", "311:142"},
                {"WorkOnHelment_Target", "461:142"},
                {"WorkOnGlove_Target", "461:250"},
                {"WorkOnShieldCloth_Target", "612:250"},
                {"WorkTurnOff_Target", "461:468"},
                {"WorkSmokeHere_Target", "612:468"}
            };

            Dictionary<string, string> numbers3 = new Dictionary<string, string>
            {
                {"BangBang_Target", "373:101"},
                {"DagerousFluid_Target", "487:101"},
                {"ElectricAlarm_Target", "373:283"},
                {"HeyItsDangeroud_Target", "487:283"},
                {"Okislitel_Target", "724:283"},
                {"RadPole_Target", "260:462"},
                {"AlarmCold_Target", "733:462"},
                {"TooLongName_Target", "852:462"}
            };

            Dictionary<string, string> numbers4 = new Dictionary<string, string>
            {
                {"DidntSmoke_Target", "338:101"},
                {"Unbeark_Target", "555:101"},
                {"NoAccess_Target", "338:232"},
                {"DontTouch_Target", "555:232"},
                {"DidntOn_Target", "772:232"},
                {"Prohod_Target", "338:369"},
                {"DidntUsePhone_Target", "668:369"},
                {"Metal_Target", "338:504"},
                {"NePodhodit_Target", "555:504"}
            };

            targets.Add(new utils.TargetImages(img1, numbers));
            targets.Add(new utils.TargetImages(img2, numbers2));
            targets.Add(new utils.TargetImages(img3, numbers3));
            targets.Add(new utils.TargetImages(img4, numbers4));

            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream fs = new FileStream("targets.dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, targets);
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
            TimeLabe.Text = delta.ToString(@"mm\:ss");
        }
    }
}
