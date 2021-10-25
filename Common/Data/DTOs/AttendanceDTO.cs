namespace Common.Data
{
    public class AttendanceDTO
    {
        public int? AttendanceId { get; set; }
        public CourseDTO Course { get; set; }
        public StudentDTO Student { get; set; }
        public string Date { get; set; }
        public string CheckInTime { get; set; }
        public string CheckOutTime { get; set; }

        public override string ToString()
        {
            string str = $"AttendanceId: {AttendanceId}, Date: {Date}, CheckInTime: {CheckInTime}, CheckOutTime: {CheckOutTime}";
            if (Course != null) str += $",\n\tCourse: {Course}";
            if (Student != null) str += $",\n\tStudent: {Student}";
            return str;
        }
    }
}
