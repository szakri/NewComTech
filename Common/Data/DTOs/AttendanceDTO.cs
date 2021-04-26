using System;

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
    }
}
