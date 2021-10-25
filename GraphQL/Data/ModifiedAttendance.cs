using System.ComponentModel.DataAnnotations;

namespace GraphQL.Data
{
    public class ModifiedAttendance
    {
        [Required]
        public int AttendanceId { get; set; }

        public string Date { get; set; }

        public string CheckInTime { get; set; }

        public string CheckOutTime { get; set; }
    }
}
