using System.ComponentModel.DataAnnotations;

namespace GraphQL.Data
{
    public class NewSubject
    {
        [Required]
        public string Name { get; set; }
    }
}
