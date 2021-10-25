﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    public class Subject
    {
        public int SubjectId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public virtual List<Course> Courses { get; set; }
    }
}