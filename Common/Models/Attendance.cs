﻿namespace Common.Models
{
    public class Attendance
    {
        public int AttendanceId { get; set; }

        public int? CourseId { get; set; }

        public Course Course { get; set; }

        public int? StudentId { get; set; }

        public Student Student { get; set; }

        public string Date { get; set; }

        public string CheckInTime { get; set; }

        public string CheckOutTime { get; set; }
    }
}
