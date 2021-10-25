using System.ComponentModel.DataAnnotations;

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
