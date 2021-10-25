using System.ComponentModel.DataAnnotations;

namespace GraphQL.Data
{
    public class ModifiedSubject
    {
        [Required]
        public int SubjectId { get; set; }

        public string Name { get; set; }
    }
}
