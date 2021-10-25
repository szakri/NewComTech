using AutoMapper;
using Common.Data;
using Common.Models;
using GraphQL.Data;
using HotChocolate;
using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQL.Services
{
    public class Mutation
    {
        private readonly IMapper _mapper;

        public Mutation(IMapper mapper)
        {
            _mapper = mapper;
        }

        [UseDbContext(typeof(SchoolContext))]
        public async Task<Student> AddStudent([ScopedService] SchoolContext context, NewStudent inputStudent)
        {
            var student = _mapper.Map<Student>(inputStudent);
            context.Students.Add(student);
            await context.SaveChangesAsync();
            return student;
        }

        [UseDbContext(typeof(SchoolContext))]
        public async Task<Student> ModifyStudent([ScopedService] SchoolContext context, ModifiedStudent inputStudent)
        {
            context.Entry(_mapper.Map<Student>(inputStudent)).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!context.Students.Any(e => e.StudentId == inputStudent.StudentId))
                {
                    throw new ArgumentException("Unsuccessfull modification!");
                }
                else
                {
                    throw;
                }
            }

            return await context.Students.FirstOrDefaultAsync(s => s.StudentId == inputStudent.StudentId);
        }

        [UseDbContext(typeof(SchoolContext))]
        public async Task<Student> DeleteStudent([ScopedService] SchoolContext context, int id)
        {
            var student = await context.Students.FindAsync(id);
            if (student == null)
            {
                throw new ArgumentException("Wrong ID!");
            }

            context.Students.Remove(student);
            await context.SaveChangesAsync();

            return student;
        }

        [UseDbContext(typeof(SchoolContext))]
        public async Task<Course> AddCourse([ScopedService] SchoolContext context, NewCourse inputCourse)
        {
            var course = _mapper.Map<Course>(inputCourse);
            context.Courses.Add(course);
            await context.SaveChangesAsync();
            return course;
        }

        [UseDbContext(typeof(SchoolContext))]
        public async Task<Course> ModifyCourse([ScopedService] SchoolContext context, ModifiedCourse inputCourse)
        {
            context.Entry(_mapper.Map<Course>(inputCourse)).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!context.Courses.Any(e => e.CourseId == inputCourse.CourseId))
                {
                    throw new ArgumentException("Unsuccessfull modification!");
                }
                else
                {
                    throw;
                }
            }

            return await context.Courses.FirstOrDefaultAsync(s => s.CourseId == inputCourse.CourseId);
        }

        [UseDbContext(typeof(SchoolContext))]
        public async Task<Course> DeleteCourse([ScopedService] SchoolContext context, int id)
        {
            var course = await context.Courses.FindAsync(id);
            if (course == null)
            {
                throw new ArgumentException("Wrong ID!");
            }

            context.Courses.Remove(course);
            await context.SaveChangesAsync();

            return course;
        }

        [UseDbContext(typeof(SchoolContext))]
        public async Task<Subject> AddSubject([ScopedService] SchoolContext context, NewSubject inputSubject)
        {
            var subject = _mapper.Map<Subject>(inputSubject);
            context.Subjects.Add(subject);
            await context.SaveChangesAsync();
            return subject;
        }

        [UseDbContext(typeof(SchoolContext))]
        public async Task<Subject> ModifySubject([ScopedService] SchoolContext context, ModifiedSubject inputSubject)
        {
            context.Entry(_mapper.Map<Subject>(inputSubject)).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!context.Subjects.Any(e => e.SubjectId == inputSubject.SubjectId))
                {
                    throw new ArgumentException("Unsuccessfull modification!");
                }
                else
                {
                    throw;
                }
            }

            return await context.Subjects.FirstOrDefaultAsync(s => s.SubjectId == inputSubject.SubjectId);
        }

        [UseDbContext(typeof(SchoolContext))]
        public async Task<Subject> DeleteSubject([ScopedService] SchoolContext context, int id)
        {
            var subject = await context.Subjects.FindAsync(id);
            if (subject == null)
            {
                throw new ArgumentException("Wrong ID!");
            }

            context.Subjects.Remove(subject);
            await context.SaveChangesAsync();

            return subject;
        }

        [UseDbContext(typeof(SchoolContext))]
        public async Task<Attendance> AddAttendance([ScopedService] SchoolContext context, NewAttendance inputAttendance)
        {
            var attendance = _mapper.Map<Attendance>(inputAttendance);
            context.Attendances.Add(attendance);
            await context.SaveChangesAsync();
            return attendance;
        }

        [UseDbContext(typeof(SchoolContext))]
        public async Task<Attendance> ModifyAttendance([ScopedService] SchoolContext context, ModifiedAttendance inputAttendance)
        {
            context.Entry(_mapper.Map<Attendance>(inputAttendance)).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!context.Attendances.Any(e => e.AttendanceId == inputAttendance.AttendanceId))
                {
                    throw new ArgumentException("Unsuccessfull modification!");
                }
                else
                {
                    throw;
                }
            }

            return await context.Attendances.FirstOrDefaultAsync(s => s.AttendanceId == inputAttendance.AttendanceId);
        }

        [UseDbContext(typeof(SchoolContext))]
        public async Task<Attendance> DeleteAttendance([ScopedService] SchoolContext context, int id)
        {
            var attendance = await context.Attendances.FindAsync(id);
            if (attendance == null)
            {
                throw new ArgumentException("Wrong ID!");
            }

            context.Attendances.Remove(attendance);
            await context.SaveChangesAsync();

            return attendance;
        }
    }
}
