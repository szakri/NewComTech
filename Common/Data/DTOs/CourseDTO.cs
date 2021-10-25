namespace Common.Data
{
    public class CourseDTO
    {
        public int? CourseId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Day { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public override string ToString()
        {
            return $"CourseId: {CourseId}, Name: {Name}, Type: {Type}, Day: {Day}, From: {From}, To: {To}";
        }
    }
}
