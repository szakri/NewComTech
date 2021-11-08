using Common.Data;
using HotChocolate.Types;
using HotChocolate;
using System.Collections.Generic;
using HotChocolate.Data;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace GraphQL.Services
{
    public class Query
    {
        [UseDbContext(typeof(SchoolContext))]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Student> GetStudents([ScopedService] SchoolContext context) => context.Students;

        [UseDbContext(typeof(SchoolContext))]
        [UseProjection]
        public async Task<Student> GetStudentAsync([ScopedService] SchoolContext context, int id) =>
            await context.Students.Include(s => s.Courses).ThenInclude(c => c.Subject).FirstOrDefaultAsync(s => s.StudentId == id);

        [UseDbContext(typeof(SchoolContext))]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Course> GetCourses([ScopedService] SchoolContext context) => context.Courses;

        [UseDbContext(typeof(SchoolContext))]
        public async Task<Course> GetCourse([ScopedService] SchoolContext context, int id) =>
            await context.Courses.Include(c => c.Subject).Include(c => c.Students).FirstOrDefaultAsync(c => c.CourseId == id);

        [UseDbContext(typeof(SchoolContext))]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Subject> GetSubjects([ScopedService] SchoolContext context) => context.Subjects;

        [UseDbContext(typeof(SchoolContext))]
        public async Task<Subject> GetSubject([ScopedService] SchoolContext context, int id) =>
            await context.Subjects.Include(s => s.Courses).FirstOrDefaultAsync(s => s.SubjectId == id);

        [UseDbContext(typeof(SchoolContext))]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Attendance> GetAttendances([ScopedService] SchoolContext context) => context.Attendances;

        [UseDbContext(typeof(SchoolContext))]
        public async Task<Attendance> GetAttendance([ScopedService] SchoolContext context, int id) =>
            await context.Attendances.Include(a => a.Student).Include(a => a.Course).ThenInclude(c => c.Subject).FirstOrDefaultAsync(a => a.AttendanceId == id);
    }
}
