using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Windows8Library;

namespace Weekly_Schedule
{
    class Graphix
    {
        public bool state { get; set; } = false;
        public static Color GetAccentColor()
        {
            System.Windows.Media.Color c = Windows8Library.ColorFunctions.GetImmersiveColor(ImmersiveColors.ImmersiveStartHoverBackground);
            return Color.FromArgb(c.A, c.R, c.G, c.B);
        }

        public static void UpdateColor(Panel panel, Color c, int duration = 30)
        {
            foreach (Control item in panel.Controls)
            {
                if (item is Button)
                {
                    try
                    {
                        Button b = (Button)item;
                        b.ForeColor = Color.White;
                        MetroFramework.Animation.ColorBlendAnimation Animation
                            = new MetroFramework.Animation.ColorBlendAnimation();
                        Animation.Start(b, "BackColor", c, duration);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public static void UpdatePanelColor(Panel panel, Color c, int duration = 30)
        {
            try
            {
                Panel b = (Panel)panel;
                b.ForeColor = Color.White;
                MetroFramework.Animation.ColorBlendAnimation Animation
                    = new MetroFramework.Animation.ColorBlendAnimation();
                Animation.Start(b, "BackColor", c, duration);
            }
            catch (Exception)
            {
            }
        }


        public static int width { get; set; } = 50;
        public static int Height { get; set; } = 25;
        public static int Gap { get; set; } = 5;
        public Weekly_Schedule.Days DayofWeek { get; set; }
        public List<Course> TodaysCourses = new List<Course>();
        public List<Button> Buttons = new List<Button>();
        public static Color ActiveColor { get; set; } = Color.Gray;
        Panel Panel;


        public Graphix(Panel Panel, Weekly_Schedule.Days Day)
        {
            this.DayofWeek = Day;
            width = Panel.Width;
            Height = Panel.Height / 17;
            foreach (Course item in CourseManager.MyCourses)
            {
                foreach (Weekly_Schedule.Days item2 in item.CourseDays)
                {
                    if (DayofWeek == item2)
                    {
                        if (!TodaysCourses.Contains(item))
                        {
                            TodaysCourses.Add(item);
                        }
                    }
                }
            }
            int[] index = new int[TodaysCourses.Count];

            for (int i = 0; i < TodaysCourses.Count; i++)
            {
                Button b = new Button();
                Buttons.Add(b);
                Panel.Controls.Add(b);
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 0;
                b.FlatAppearance.BorderColor = Color.FromArgb(0, 0, 0, 0);

                for (int j = 0; j < TodaysCourses[i].Times.Count; j++)
                {
                    if (TodaysCourses[i].CourseDays[j] == this.DayofWeek)
                    {
                        index[i] = j;
                    }
                }
                int heightCount = 2 * (TodaysCourses[i].Times[index[i]].EHour - TodaysCourses[i].Times[index[i]].SHour) +
                    (TodaysCourses[i].Times[index[i]].EMinute - TodaysCourses[i].Times[index[i]].SMinute) / 30;


                int posCount = 2 * (TodaysCourses[i].Times[index[i]].SHour - 8) +
                    (TodaysCourses[i].Times[index[i]].SMinute - 30) / 30;

                b.Size = GetButtonSize(heightCount);
                b.Location = new Point(0, posCount * Height);
                b.BackColor = ActiveColor;
                b.Text = TodaysCourses[i].CourseCode + "\n" + TodaysCourses[i].CourseTitle + "\n" + TodaysCourses[i].Buildings[index[i]] + " - " + TodaysCourses[i].Classes[index[i]]
                    + "\n" + TodaysCourses[i].CourseInstructor;
            }

            List<Button> SortedList = Buttons.OrderBy(o => o.Location.Y).ToList();
            Buttons = SortedList;


            //List<Course> SortedList2 = TodaysCourses.OrderBy(o => o.Times[index[i]].SHour).ToList();
            //TodaysCourses = SortedList2;

            this.Panel = Panel;
        }

        public Course GetByOrder(int order = 1)
        {
            return TodaysCourses[order - 1];
        }

        public Size GetButtonSize(int DurationByHalfOur)
        {
            return new Size(width, DurationByHalfOur * Height - Gap);
        }
    }
}
