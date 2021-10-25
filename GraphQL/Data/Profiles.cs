using AutoMapper;
using Common.Models;

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
