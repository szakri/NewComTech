using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseDataGen.Models
{
    public class Subject
    {
        public int SubjectID { get; set; }
        public string Name { get; set; }

        public ICollection<Course> Courses { get; set; }
    }
}
