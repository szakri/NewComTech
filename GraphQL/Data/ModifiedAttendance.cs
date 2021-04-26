using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQL.Data
{
    public class ModifiedAttendance
    {
        [Required]
        public int AttendanceId { get; set; }

        public string? Date { get; set; }

        public string? CheckInTime { get; set; }

        public string? CheckOutTime { get; set; }
    }
}
