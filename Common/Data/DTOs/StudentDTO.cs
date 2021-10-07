namespace Common.Data
{
    public class StudentDTO
    {
        public int? StudentId { get; set; }
        public string Neptun { get; set; }
        public string Name { get; set; }
        public string DayOfBirth { get; set; }

        public override string ToString()
        {
            return $"StudentId: {StudentId}, Neptun: {Neptun}, Name: {Name}, DayOfBirth: {DayOfBirth}";
        }
    }
}
