using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQL.Data
{
    public class NewStudent
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string DayOfBirth { get; set; }

        [Required]
        public string Neptun { get; set; }
    }
}
