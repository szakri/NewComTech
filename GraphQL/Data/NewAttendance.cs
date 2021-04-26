using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQL.Data
{
    public class NewAttendance
    {
        public string? Date { get; set; }

        public string? CheckInTime { get; set; }

        public string? CheckOutTime { get; set; }
    }
}
