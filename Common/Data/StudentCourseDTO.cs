using System.Collections.Generic;
using System;

namespace Common.Data
{
    public class CourseStudentDTO
    {
        public string Neptun { get; set; }
        public string Name { get; set; }
        public DateTime DayOfBirth { get; set; }
        public IEnumerable<CourseDTO> Course { get; set; }
    }
}
