using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQL.Data
{
    public class NewCourse
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; }

        public DayOfWeek? Day { get; set; }

        public string? From { get; set; }

        public string? To { get; set; }
    }
}
