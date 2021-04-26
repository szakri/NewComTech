using System;

namespace Common.Data
{
    public class CourseDTO
    {
        public int? CourseId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DayOfWeek Day { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}
