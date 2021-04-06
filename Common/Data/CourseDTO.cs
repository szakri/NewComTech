using System;

namespace Common.Data
{
    public class CourseDTO
    {
        public int CourseID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DayOfWeek Day { get; set; }
        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }
    }
}
