using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQL.Data
{
    public class ModifiedStudent
    {
        [Required]
        public int StudentId { get; set; }

        public string Name { get; set; }

        public string DayOfBirth { get; set; }

        public string Neptun { get; set; }
    }
}
