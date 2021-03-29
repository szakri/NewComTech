using AutoMapper;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Data
{
    public class CourseSubjectProfile : Profile
    {
        public CourseSubjectProfile()
        {
            CreateMap<Course, CourseSubjectDTO>();
        }
    }
}
