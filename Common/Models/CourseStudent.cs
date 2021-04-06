using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Models
{
    public class CourseStudent
    {
        public int CoursesCourseID { get; set; }
        public Course Course { get; set; }
        public int StudentsStudentID { get; set; }
        public Student Student { get; set; }
    }
}
