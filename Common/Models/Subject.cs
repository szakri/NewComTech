using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models
{
    public class Subject
    {
        public int SubjectID { get; set; }
        public string Name { get; set; }

        [ForeignKey("CourseId")]
        public ICollection<Course> Courses { get; set; }
    }
}
