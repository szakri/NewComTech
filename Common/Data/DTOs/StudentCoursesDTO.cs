using System.Collections.Generic;
using System;

namespace Common.Data
{
    public class StudentCoursesDTO
    {
        public int StudentId { get; set; }
        public string Neptun { get; set; }
        public string Name { get; set; }
        public string DayOfBirth { get; set; }
        public List<CourseDTO> Courses { get; set; }

        public override string ToString()
        {
            string str = $"StudentId: {StudentId}, Neptun: {Neptun}, Name: {Name}, DayOfBirth: {DayOfBirth}";
            foreach (var c in Courses)
            {
                str += "\n\t" + c.ToString();
            }
            return str;
        }
    }
}
