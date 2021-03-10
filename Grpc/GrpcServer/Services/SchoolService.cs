using AutoMapper;
using Common.Data;
using Common.Models;
using Grpc.Core;
using GrpcServer.Protos;
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

        public override async Task GetCourses(EmptyMessage request, IServerStreamWriter<CourseDTO> responseStream, ServerCallContext context)
        {
            List<Course> responses = _context.Courses.ToList();
            foreach (var response in responses)
            {
                await responseStream.WriteAsync(_mapper.Map<CourseDTO>(response));
            }
        }

        public override async Task GetSubjects(EmptyMessage request, IServerStreamWriter<SubjectDTO> responseStream, ServerCallContext context)
        {
            List<Subject> responses = _context.Subjects.ToList();
            foreach (var response in responses)
            {
                await responseStream.WriteAsync(_mapper.Map<SubjectDTO>(response));
            }
        }
    }
}
