using System;
using System.ComponentModel.DataAnnotations;

namespace GraphQL.Data
{
    public class ModifiedCourse
    {
        [Required]
        public int CourseId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public DayOfWeek? Day { get; set; }

        public string From { get; set; }

        public string To { get; set; }
    }
}
