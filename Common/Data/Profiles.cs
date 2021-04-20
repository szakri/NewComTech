using AutoMapper;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Data
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<Course, CourseDTO>();
            CreateMap<CourseDTO, Course>();

            CreateMap<Course, CourseSubjectDTO>();

            CreateMap<Student, StudentCoursesDTO>();

            CreateMap<Student, StudentDTO>();
            CreateMap<StudentDTO, Student>();

            CreateMap<Student, StudentQRCodeDTO>();
            CreateMap<StudentQRCodeDTO, Student>();

            CreateMap<Subject, SubjectDTO>();
            CreateMap<SubjectDTO, Subject>();
        }
    }

    public class StudentCourseConverter : ITypeConverter<Student, StudentCoursesDTO>
    {
        private readonly IMapper _mapper;
        public StudentCourseConverter(IMapper mapper)
        {
            _mapper = mapper;
        }
        public StudentCoursesDTO Convert(Student source, StudentCoursesDTO destination, ResolutionContext context)
        {
            return new StudentCoursesDTO
            {
                StudentId = source.StudentId,
                Name = source.Name,
                Neptun = source.Neptun,
                DayOfBirth = source.DayOfBirth,
                Courses = _mapper.Map<List<CourseDTO>>(source.Courses)
            };
        }
    }
}
