using System.Collections.Generic;
using System;
using Common.Models;

namespace Common.Data
{
    public class StudentCoursesDTO
    {
        public int ID { get; set; }
        public string Neptun { get; set; }
        public string Name { get; set; }
        public DateTime DayOfBirth { get; set; }
        public IEnumerable<CourseDTO> Courses { get; set; }
    }
}
