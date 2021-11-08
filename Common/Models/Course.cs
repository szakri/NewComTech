using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    public class Course
    {
        public int CourseId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(10)]
        public string Type { get; set; }

        public int? Day { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public int? SubjectId { get; set; }
        public Subject Subject { get; set; }

        public ICollection<Student> Students { get; set; }
    }
}
