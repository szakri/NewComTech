using AutoMapper;
using Common.Data;
using HotChocolate.Types;
using HotChocolate;
using System.Linq;
using System.Collections.Generic;
using HotChocolate.Data;
using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace GraphQL.Services
{
    public class Query
    {
        [UseDbContext(typeof(SchoolContext))]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Student> GetStudents([ScopedService] SchoolContext context) =>
            context.Students.Include(s => s.Courses);

        [UseDbContext(typeof(SchoolContext))]
        public Student GetStudent([ScopedService] SchoolContext context, int id) =>
            context.Students.Include(s => s.Courses).FirstOrDefault(s => s.StudentId == id);

        [UseDbContext(typeof(SchoolContext))]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Course> GetCourses([ScopedService] SchoolContext context) =>
            context.Courses.Include(c => c.Subject);

        [UseDbContext(typeof(SchoolContext))]
        public Course GetCourse([ScopedService] SchoolContext context, int id) =>
            context.Courses.Include(c => c.Subject).FirstOrDefault(c => c.CourseId == id);

        [UseDbContext(typeof(SchoolContext))]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Subject> GetSubjects([ScopedService] SchoolContext context) =>
            context.Subjects;

        [UseDbContext(typeof(SchoolContext))]
        public Subject GetSubject([ScopedService] SchoolContext context, int id) =>
            context.Subjects.Find(id);

        [UseDbContext(typeof(SchoolContext))]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Attendance> GetAttendances([ScopedService] SchoolContext context) =>
            context.Attendances.Include(a => a.Course).Include(a => a.Student);

        [UseDbContext(typeof(SchoolContext))]
        public Attendance GetAttendance([ScopedService] SchoolContext context, int id) =>
            context.Attendances.Include(a => a.Course).Include(a => a.Student).FirstOrDefault(a => a.AttendanceId == id);
    }
}
