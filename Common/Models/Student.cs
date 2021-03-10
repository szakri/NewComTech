using System;
using System.Collections.Generic;

namespace Common.Models
{
    public class Student
    {
        public int StudentID { get; set; }
        public string Neptun { get; set; }
        public string Name { get; set; }
        public DateTime DayOfBirth { get; set; }

        public ICollection<CourseStudent> Courses { get; set; }
    }
}
