using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

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

        public string? From { get; set; }

        public string? To { get; set; }

        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public List<Student>? Students { get; set; }
    }
}
