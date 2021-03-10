using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models
{
    public class Course
    {
        public int CourseID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DayOfWeek Day { get; set; }
        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }
        [ForeignKey("SubjectId")]
        public Subject Subject { get; set; }

        public ICollection<CourseStudent> Students { get; set; }
    }
}
