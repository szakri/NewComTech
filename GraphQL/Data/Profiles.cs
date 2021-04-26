using AutoMapper;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQL.Data
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<ModifiedStudent, Student>();
            CreateMap<NewStudent, Student>();

            CreateMap<ModifiedCourse, Course>();
            CreateMap<NewCourse, Course>();

            CreateMap<ModifiedSubject, Subject>();
            CreateMap<NewSubject, Subject>();

            CreateMap<ModifiedAttendance, Attendance>();
            CreateMap<NewAttendance, Attendance>();
        }
    }
}
