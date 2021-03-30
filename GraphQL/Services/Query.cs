using AutoMapper;
using Common.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<StudentDTO>> GetStudents()
        {
            var students = await _context.Students.ToListAsync();
            return _mapper.Map<List<StudentDTO>>(students);
        }

        public async Task<IEnumerable<CourseDTO>> GetCourses()
        {
            var courses = await _context.Courses.ToListAsync();
            return _mapper.Map<List<CourseDTO>>(courses);
        }

        public async Task<IEnumerable<CourseSubjectDTO>> GetCoursesWithSubject()
        {
            var courses = await _context.Courses.Include(c => c.Subject).ToListAsync();
            return _mapper.Map<List<CourseSubjectDTO>>(courses);
        }

        public async Task<IEnumerable<SubjectDTO>> GetSubjects()
        {
            var subjects = await _context.Subjects.ToListAsync();
            return _mapper.Map<List<SubjectDTO>>(subjects);
        }
    }
}
