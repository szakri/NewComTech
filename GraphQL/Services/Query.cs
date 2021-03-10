using AutoMapper;
using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQL.Services
{
    public class Query
    {
        private readonly SchoolContext _context;
        private readonly IMapper _mapper;

        public Query(SchoolContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<StudentDTO> GetStudents()
        {
            return _mapper.Map<List<StudentDTO>>(_context.Students.ToList());
        }

        public List<CourseDTO> GetCourses()
        {
            return _mapper.Map<List<CourseDTO>>(_context.Courses.ToList());
        }

        public List<SubjectDTO> GetSubjects()
        {
            return _mapper.Map<List<SubjectDTO>>(_context.Subjects.ToList());
        }
    }
}
