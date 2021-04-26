using AutoMapper;
using Common.Data;
using HotChocolate.Types;
using HotChocolate;
using System.Linq;
using System.Collections.Generic;
using HotChocolate.Data;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GraphQL.Services
{
    public class Query
    {
        [UseDbContext(typeof(SchoolContext))]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public async Task<IEnumerable<Student>> GetStudents([ScopedService] SchoolContext context) =>
            await context.Students.Include(s => s.Courses).ToListAsync();

        [UseDbContext(typeof(SchoolContext))]
        public async Task<Student> GetStudent([ScopedService] SchoolContext context, int id) =>
            await context.Students.Include(s => s.Courses).FirstOrDefaultAsync(s => s.StudentId == id);

        [UseDbContext(typeof(SchoolContext))]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public async Task<IEnumerable<Course>> GetCourses([ScopedService] SchoolContext context) =>
            await context.Courses.Include(c => c.Subject).Include(c => c.Students).ToListAsync();

        [UseDbContext(typeof(SchoolContext))]
        public async Task<Course> GetCourse([ScopedService] SchoolContext context, int id) =>
            await context.Courses.Include(c => c.Subject).Include(c => c.Students).FirstOrDefaultAsync(c => c.CourseId == id);

        [UseDbContext(typeof(SchoolContext))]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public async Task<IEnumerable<Subject>> GetSubjects([ScopedService] SchoolContext context) =>
            await context.Subjects.Include(s => s.Courses).ToListAsync();

        [UseDbContext(typeof(SchoolContext))]
        public async Task<Subject> GetSubject([ScopedService] SchoolContext context, int id) =>
            await context.Subjects.Include(s => s.Courses).FirstOrDefaultAsync(s => s.SubjectId == id);

        [UseDbContext(typeof(SchoolContext))]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public async Task<IEnumerable<Attendance>> GetAttendances([ScopedService] SchoolContext context) =>
            await context.Attendances.Include(a => a.Course).Include(a => a.Student).ToListAsync();

        [UseDbContext(typeof(SchoolContext))]
        public async Task<Attendance> GetAttendance([ScopedService] SchoolContext context, int id) =>
            await context.Attendances.Include(a => a.Course).Include(a => a.Student).FirstOrDefaultAsync(a => a.AttendanceId == id);
    }
}
