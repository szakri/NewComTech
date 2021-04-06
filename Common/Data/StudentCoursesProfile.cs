using AutoMapper;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Data
{
    public class StudentCoursesProfile : Profile
    {
        public StudentCoursesProfile()
        {
            CreateMap<Student, StudentCoursesDTO>();
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
                ID = source.StudentID,
                Name = source.Name,
                Neptun = source.Neptun,
                DayOfBirth = source.DayOfBirth,
                Courses = _mapper.Map<List<CourseDTO>>(source.Courses)
            };
        }
    }
}
