using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Weekly_Schedule
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Button[] Buttons = new Button[17];
        Graphix[] gg = new Graphix[5];
        Graphix d;
        Graphix d1;
        Graphix d2;
        Graphix d3;
        Graphix d4;

        private void Form1_Load(object sender, EventArgs e)
        {
            //CourseManager.UpdateCache();

            // OLD
            //Course.Deserialize();
            //foreach (CourseXml item in Course.CoursesXml)
            //{
            //    CourseManager.AddCourse(CourseManager.GetCourseByCRN(item.Code, item.CRN));
            //}

            if (!File.Exists("AllProfiles.xml"))
            {
                Updater u = new Updater();
                u.ShowDialog();
            }
            CourseManager.Deserialize();
            string[] crn_list = Course.LoadCRNList();
            foreach (var item in crn_list)
            {
                Console.WriteLine(item);
            }
            foreach (var item in crn_list)
            {
                var c = CourseManager.GetCourseByCRNLocal(item);
                Console.Write("Adding Course: ");
                Console.WriteLine(c);
                CourseManager.AddCourse(c);
            }

            SystemEvents.UserPreferenceChanging += SystemEvents_UserPreferenceChanging;
            d = new Graphix(panel1, Days.Monday);
            d1 = new Graphix(panel2, Days.Tuesday);
            d2 = new Graphix(panel3, Days.Wednesday);
            d3 = new Graphix(panel4, Days.Thursday);
            d4 = new Graphix(panel5, Days.Friday);

            for (int i = 0; i < 17; i++)
            {
                Buttons[i] = new Button();
                buttonsP.Controls.Add(Buttons[i]);
                Buttons[i].Size = new Size(buttonsP.Width, Graphix.Height);
                Buttons[i].Location = new Point(0, (Graphix.Height) * i);
                Buttons[i].FlatAppearance.BorderSize = 0;
                Buttons[i].FlatAppearance.BorderColor = Color.FromArgb(0, 0, 0, 0);
                Buttons[i].FlatStyle = FlatStyle.Flat;
                string AfterText = (8 + (i + 2) / 2).ToString() + ":" + (30 * ((i + 2) % 2)).ToString();
                Buttons[i].Text = (8 + (i + 1) / 2).ToString() + ":" + (30 * ((i + 1) % 2)).ToString() + "-" + AfterText;
            }
            panel6.BackColor = Color.FromArgb(255, 30, 30, 30);
            Graphix.UpdatePanelColor(panel8, Graphix.GetAccentColor());
            foreach (Control item in panel6.Controls)
            {
                if (item is Panel)
                {
                    Panel p = (Panel)item;
                    Graphix.UpdateColor(p, Graphix.GetAccentColor());
                }
            }
            this.Size = panel8.Size;

            foreach (Button item in panel7.Controls)
            {
                item.Click += Item_Click;

            }
            gg[0] = d;
            gg[1] = d1;
            gg[2] = d2;
            gg[3] = d3;
            gg[4] = d4;
            List<Panel> lop = GetPanels(this);
            foreach (Panel item in lop)
            {
                item.MouseDown += Item_MouseDown;
            }
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Item_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {

                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x20000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        public List<Panel> GetPanels(Control c)
        {
            List<Panel> p = new List<Panel>();
            foreach (Control item in c.Controls)
            {
                if (item is Panel)
                {
                    Panel pp = (Panel)item;
                    p.Add(pp);
                    List<Panel> p2 = GetPanels(item);
                    foreach (Panel item2 in p2)
                    {
                        p.Add(item2);
                    }
                }
            }
            return p;
        }

        private void Item_Click(object sender, EventArgs e)
        {
            Button item = (Button)sender;
            string index = (string)item.Tag.ToString();
            switch (index)
            {
                case "1": Disable(d); break;
                case "2": Disable(d1); break;
                case "3": Disable(d2); break;
                case "4": Disable(d3); break;
                case "5": Disable(d4); break;
            }
        }

        private void Disable(Graphix c)
        {
            foreach (Graphix item in gg)
            {
                if (item != c) item.state = false;
            }

            c.state = !c.state;

            if (c.state)
            {
                foreach (Button item in Buttons)
                {
                    item.Enabled = false;
                }
                for (int j = 0; j < c.Buttons.Count; j++)
                {
                    int k = c.Buttons[j].Location.Y / Graphix.Height;
                    int l = c.Buttons[j].Height / Graphix.Height;

                    for (int i = k; i < k + l + 1; i++)
                    {

                        Buttons[i].Enabled = true;
                    }
                }
            }
            else
            {
                foreach (Button item in Buttons)
                {
                    item.Enabled = true;
                }
            }

        }

        private void SystemEvents_UserPreferenceChanging(object sender, UserPreferenceChangingEventArgs e)
        {
            Graphix.UpdatePanelColor(panel8, Graphix.GetAccentColor());
            foreach (Control item in panel6.Controls)
            {
                if (item is Panel)
                {
                    Panel p = (Panel)item;
                    Graphix.UpdateColor(p, Graphix.GetAccentColor());
                }
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Updater u = new Updater();
            u.ShowDialog();
        }
    }
}
