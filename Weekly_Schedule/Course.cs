using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Weekly_Schedule
{
    public enum Days
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }

    public class CourseXml
    {
        public string Code { get; set; } = " ";
        public string CRN { get; set; } = " ";

        public CourseXml()
        {

        }
    }

    class DaysConversion
    {
        public static Days ToDays(string Day)
        {
            if (Day == "Pazatesi") return Days.Monday;
            else if (Day == "Salı") return Days.Tuesday;
            else if (Day == "Çarşamba") return Days.Wednesday;
            else if (Day == "Perşembe") return Days.Thursday;
            else if (Day == "Cuma") return Days.Friday;
            else if (Day == "Cumartesi") return Days.Saturday;
            else if (Day == "Pazar") return Days.Sunday;
            else { return Days.Monday; }
        }
    }

    public class Course
    {
        public string CRN { get; set; }
        public string CourseTitle { get; set; }
        public string CourseCode { get; set; }
        public string CourseInstructor { get; set; }
        public List<string> Buildings { get; set; } = new List<string>();
        public List<string> Classes { get; set; } = new List<string>();
        public List<Days> CourseDays { get; set; } = new List<Days>();
        public List<CourseCustomTime> Times { get; set; } = new List<CourseCustomTime>();

        public static List<CourseXml> CoursesXml = new List<CourseXml>();

        public static void Deserialize()
        {
            CoursesXml.Clear();
            StreamReader Reader = new StreamReader("Profiles.xml");

            XmlSerializer xml = new XmlSerializer(CoursesXml.GetType());
            List<CourseXml> Course = (List<CourseXml>)xml.Deserialize(Reader);
            Reader.Close();
            foreach (CourseXml item in Course)
            {
                CoursesXml.Add(item);
            }
        }

        public static string[] LoadCRNList()
        {
            StreamReader Reader = new StreamReader("Crn_list.txt");
            string[] lines = Reader.ReadToEnd().Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Trim();
            }
            Reader.Close();

            return new List<string>(lines).FindAll(item => item != "").ToArray();
        }


        public override string ToString()
        {
            string[] items = new string[Times.Count];

            for (int i = 0; i < Times.Count; i++)
            {
                items[i] = Times[i].ToString();
            }

            return string.Format("Title: {0}, Code: {1}, Instructor: {2}," +
                " Buildings: {3}, Classes: {4}, Days: {5}, Times: {6}",
                CourseTitle, CourseCode, CourseInstructor,
                string.Join(",", Buildings.ToArray()),
                string.Join(",", Classes.ToArray()),
                string.Join(",", CourseDays.ToArray()),
                string.Join(",", items)
                );
        }
    }
}
