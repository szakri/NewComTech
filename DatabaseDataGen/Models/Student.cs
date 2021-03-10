using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseDataGen.Models
{
    public class Student
    {
        public int StudentID { get; set; }
        public string Neptun { get; set; }
        public string Name { get; set; }
        public DateTime DayOfBirth { get; set; }

        public ICollection<Course> Courses { get; set; }
    }
}
