using AutoMapper;
using Common.Data;
using Common.Models;
using Grpc.Core;
using GrpcServer.Protos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcServer.Services
{
    public class SchoolService : School.SchoolBase
    {
        private readonly SchoolContext _context;
        private readonly IMapper _mapper;

        public SchoolService(SchoolContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public override async Task GetStudents(EmptyMessage request, IServerStreamWriter<StudentDTO> responseStream, ServerCallContext context)
        {
            List<Student> responses = _context.Students.ToList();
            foreach (var response in responses)
            {
                await responseStream.WriteAsync(_mapper.Map<StudentDTO>(response));
            }
        }

        public override async Task<StudentDTO> GetStudent(Id request, ServerCallContext context)
        {
            var student = await _context.Students.FindAsync(request.ID);
            if (student == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "No such student exists (ID = " + request.ID + ")!"));
            }
            return _mapper.Map<StudentDTO>(student);
        }

        public override async Task GetCourses(EmptyMessage request, IServerStreamWriter<CourseDTO> responseStream, ServerCallContext context)
        {
            List<Course> responses = _context.Courses.ToList();
            foreach (var response in responses)
            {
                await responseStream.WriteAsync(_mapper.Map<CourseDTO>(response));
            }
        }

        public override async Task<CourseDTO> GetCourse(Id request, ServerCallContext context)
        {
            var course = await _context.Courses.FindAsync(request.ID);
            if (course == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "No such course exists (ID = " + request.ID + ")!"));
            }
            return _mapper.Map<CourseDTO>(course);
        }

        public override async Task GetCoursesWithSubject(EmptyMessage request, IServerStreamWriter<CourseSubjectDTO> responseStream, ServerCallContext context)
        {
            List<Course> responses = _context.Courses.Include(c => c.Subject).ToList();
            foreach (var response in responses)
            {
                await responseStream.WriteAsync(_mapper.Map<CourseSubjectDTO>(response));
            }
        }

        public override async Task<CourseSubjectDTO> GetCourseWithSubject(Id request, ServerCallContext context)
        {
            var course = await _context.Courses.Include(c => c.Subject).FirstOrDefaultAsync(c => c.CourseID == request.ID);
            if (course == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "No such course exists (ID = " + request.ID + ")!"));
            }
            return _mapper.Map<CourseSubjectDTO>(course);
        }

        public override async Task GetSubjects(EmptyMessage request, IServerStreamWriter<SubjectDTO> responseStream, ServerCallContext context)
        {
            List<Subject> responses = _context.Subjects.ToList();
            foreach (var response in responses)
            {
                await responseStream.WriteAsync(_mapper.Map<SubjectDTO>(response));
            }
        }

        public override async Task<SubjectDTO> GetSubject(Id request, ServerCallContext context)
        {
            var subject = await _context.Subjects.FindAsync(request.ID);
            if (subject == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "No such subject exists (ID = " + request.ID + ")!"));
            }
            return _mapper.Map<SubjectDTO>(subject);
        }
    }
}
