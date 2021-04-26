using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQL.Data
{
    public class ModifiedCourse
    {
        [Required]
        public int CourseId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public DayOfWeek? Day { get; set; }

        public string? From { get; set; }

        public string? To { get; set; }
    }
}
