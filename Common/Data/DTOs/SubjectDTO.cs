namespace Common.Data
{
    public class SubjectDTO
    {
        public int? SubjectId { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"SubjectId: {SubjectId}, Name: {Name}";
        }
    }
}
