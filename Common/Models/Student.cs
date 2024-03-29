﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    public class Student
    {
        public int StudentId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string DayOfBirth { get; set; }

        [Required, MaxLength(6)]
        public string Neptun { get; set; }

        public byte[] QRCode { get; set; }

        public ICollection<Course> Courses { get; set; }
    }
}
