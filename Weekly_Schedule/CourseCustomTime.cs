namespace Weekly_Schedule
{
    public class CourseCustomTime
    {
        public int SHour { get; set; }
        public int SMinute { get; set; }
        public int EHour { get; set; }
        public int EMinute { get; set; }

        public CourseCustomTime()
        {

        }

        public CourseCustomTime(int SHour, int SMinute, int EHour, int EMinute)
        {
            this.SHour = SHour;
            this.SMinute = SMinute;
            this.EHour = EHour;
            this.EMinute = EMinute;
        }

        public override string ToString()
        {
            return SHour + ":" + SMinute + "//" + EHour + ":" + EMinute;
        }
    }
}
