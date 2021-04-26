using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQL.Data
{
    public class ModifiedSubject
    {
        [Required]
        public int SubjectId { get; set; }

        public string Name { get; set; }
    }
}
