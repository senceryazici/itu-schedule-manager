using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Serialization;

namespace Weekly_Schedule
{
    class CourseManager
    {
        public static List<Course> MyCourses = new List<Course>();
        public static List<Course> AllCourses = new List<Course>();
        private static string DownloadingCode = "";
        public static List<string> CourseCodeList = new List<string>();

        public static string GetDownloadingCode()
        {
            return DownloadingCode;
        }

        public static void AddCourse(Course course)
        {
            MyCourses.Add(course);
        }

        public static Course GetCourseByCRN(string Code, string CRN)
        {
            List<List<string>> Table = GetAllByCourseCode(Code);
            List<string> CourseFounded = new List<string>();
            bool Founded = false;
            foreach (List<string> item in Table)
            {
                if (!Founded)
                {
                    if (item[0] == CRN)
                    {
                        CourseFounded = item;
                        Founded = true;
                    }
                }
            }

            if (Founded)
            {
                string FullCourseCode = CourseFounded[1];
                string CourseName = CourseFounded[2];
                string ins = CourseFounded[3];
                List<string> Buildings = new List<string>();
                int index = CourseFounded[4].Length / 3;

                for (int i = 0; i < index; i++)
                {
                    Buildings.Add(CourseFounded[4].Substring(i * 3, 3));
                    //Console.WriteLine("asd  " + Buildings[i]);
                }

                string[] Days = CourseFounded[5].Split(' ');
                string[] Times = CourseFounded[6].Split(' ');

                string[] Classes = CourseFounded[7].Split(' ');
                int LessonsCountInWeek = 0;

                int countOfTimes = Times.Length;
                List<CourseCustomTime> TimesList = new List<CourseCustomTime>();

                for (int i = 0; i < countOfTimes; i++)
                {
                    string[] Time = Times[i].Split('/');

                    int BH = int.Parse(Time[0].Substring(0, 2));
                    int BM = int.Parse(Time[0].Substring(2, 2));
                    int EH = int.Parse(Time[1].Substring(0, 2));
                    int EM = int.Parse(Time[1].Substring(2, 2));
                    TimesList.Add(new CourseCustomTime(BH, BM, EH, EM));
                }

                if (Buildings.Count == Days.Length && TimesList.Count == Classes.Length && TimesList.Count == Days.Length)
                {
                    LessonsCountInWeek = Buildings.Count;

                    Course course = new Course();
                    course.CourseTitle = CourseName;
                    course.CourseCode = FullCourseCode;
                    course.CourseInstructor = ins;


                    for (int i = 0; i < LessonsCountInWeek; i++)
                    {
                        course.CourseDays.Add(DaysConversion.ToDays(Days[i]));
                        course.Buildings.Add(Buildings[i]);
                        course.Times.Add(TimesList[i]);
                        course.Classes.Add(Classes[i]);
                    }
                    return course;
                }
            }

            return new Course();
        }

        public static string ToString()
        {
            string[] Array = new string[MyCourses.Count];
            for (int i = 0; i < MyCourses.Count; i++)
            {
                Array[i] = MyCourses[i].ToString();
            }
            return string.Join("\n\n", Array);
        }

        public static Course GetCourseByCRNLocal(string CRN)
        {
            return AllCourses.Find(item => item.CRN == CRN);
        }


        private static List<List<string>> GetAllByCourseCode(string Code)
        {
            WebClient webClient = new WebClient();
            string page = webClient.DownloadString("http://www.sis.itu.edu.tr/tr/ders_programlari/LSprogramlar/prg.php?fb=" + Code);

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(page);

            return doc.DocumentNode.SelectSingleNode("//table[@class='dersprg']")
            .Descendants("tr")
            .Skip(1)
            .Where(tr => tr.Elements("td").Count() > 1)
            .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
            .ToList();
        }

        public static void Serialize()
        {
            StreamWriter Writer = new StreamWriter("AllProfiles.xml");

            XmlSerializer xml = new XmlSerializer(AllCourses.GetType());
            xml.Serialize(Writer, AllCourses);
            Writer.Close();
        }

        public static void Deserialize()
        {
            AllCourses.Clear();
            StreamReader Reader = new StreamReader("AllProfiles.xml");

            XmlSerializer xml = new XmlSerializer(AllCourses.GetType());
            List<Course> Course = (List<Course>)xml.Deserialize(Reader);
            Reader.Close();
            foreach (Course item in Course)
            {
                AllCourses.Add(item);
            }
        }

        public static void GetAllCourses(Action<string> f = null)
        {
            AllCourses.Clear();
            WebClient webClient = new WebClient();

            string page = webClient.DownloadString("http://www.sis.itu.edu.tr/tr/ders_programlari/LSprogramlar/prg.php");
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(page);

            var node = doc.DocumentNode.SelectSingleNode("//select");

            List<string> course_codes = new List<string>();
            foreach (var item in node.Descendants("option"))
            {
                if (item.Attributes["value"].Value.Trim() != "")
                {
                    course_codes.Add(item.Attributes["value"].Value);
                }
            }
            CourseCodeList = course_codes.ToList();
            foreach (var item in course_codes)
            {

                System.Console.WriteLine("Downloading Courses for Code:" + item);
                CourseManager.DownloadingCode = item;
                if (f != null)
                {
                    f(item);
                }
                List<List<string>> Table = GetAllByCourseCode(item);

                foreach (List<string> element in Table)
                {
                    if (element[0] == "CRN")
                    {
                        continue;
                    }
                    List<string> CourseFounded = element;

                    string FullCourseCode = CourseFounded[1];
                    string CourseName = CourseFounded[2];
                    string ins = CourseFounded[3];
                    List<string> Buildings = new List<string>();
                    int index = CourseFounded[4].Length / 3;

                    for (int i = 0; i < index; i++)
                    {
                        Buildings.Add(CourseFounded[4].Substring(i * 3, 3));
                        //Console.WriteLine("asd  " + Buildings[i]);
                    }

                    string[] Days = CourseFounded[5].Split(' ');
                    string[] Times = CourseFounded[6].Split(' ');

                    string[] Classes = CourseFounded[7].Split(' ');
                    int LessonsCountInWeek = 0;

                    int countOfTimes = Times.Length;
                    List<CourseCustomTime> TimesList = new List<CourseCustomTime>();

                    for (int i = 0; i < countOfTimes; i++)
                    {
                        if (Times[i].Trim() == "/")
                        {
                            continue;
                        }
                        string[] Time = Times[i].Split('/');

                        int BH = int.Parse(Time[0].Substring(0, 2));
                        int BM = int.Parse(Time[0].Substring(2, 2));
                        int EH = int.Parse(Time[1].Substring(0, 2));
                        int EM = int.Parse(Time[1].Substring(2, 2));
                        TimesList.Add(new CourseCustomTime(BH, BM, EH, EM));
                    }

                    if (Buildings.Count == Days.Length && TimesList.Count == Classes.Length && TimesList.Count == Days.Length)
                    {
                        LessonsCountInWeek = Buildings.Count;

                        Course course = new Course();
                        course.CourseTitle = CourseName;
                        course.CourseCode = FullCourseCode;
                        course.CourseInstructor = ins;
                        course.CRN = element[0];

                        for (int i = 0; i < LessonsCountInWeek; i++)
                        {
                            course.CourseDays.Add(DaysConversion.ToDays(Days[i]));
                            course.Buildings.Add(Buildings[i]);
                            course.Times.Add(TimesList[i]);
                            course.Classes.Add(Classes[i]);
                        }
                        AllCourses.Add(course);
                    }

                }
            }
            System.Console.WriteLine(AllCourses.Count);
            //var options = node.Descendants("option");
            //foreach (var opt in options)
            //{
            //    System.Console.WriteLine(opt);
            //}
        }
    }
}
