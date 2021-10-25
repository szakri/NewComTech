using System.ComponentModel.DataAnnotations;

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
