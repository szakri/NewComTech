using System;
using System.ComponentModel.DataAnnotations;

namespace GraphQL.Data
{
    public class NewCourse
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; }

        public DayOfWeek? Day { get; set; }

        public string From { get; set; }

        public string To { get; set; }
    }
}
