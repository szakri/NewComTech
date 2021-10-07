using AutoMapper;
using Common.Data;
using Grpc.Core;
using GrpcServer.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcServer.Services
{
    public class StudentsService : Students.StudentsBase
    {

        private readonly SchoolContext _context;
        private readonly IMapper _mapper;

        public StudentsService(SchoolContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public override async Task GetStudents(QueryParams request, IServerStreamWriter<StudentDTO> responseStream, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.OrderBy)) request.OrderBy = "studentId";
            if (request.PageNumber == 0) request.PageNumber = 1;
            if (request.PageSize == 0) request.PageSize = 10;
            if (request.PageSize > 100) request.PageSize = 100;
            IOrderedQueryable<Student> students;
            if (string.IsNullOrEmpty(request.FilterBy)) students = _context.Students.OrderBy(request.OrderBy);
            else students = _context.Students.Where(request.FilterBy).OrderBy(request.OrderBy);
            List<Student> responses = await PaginatedList<Student>.CreateAsync(students, request.PageNumber, request.PageSize);
            foreach (var response in responses)
            {
                await responseStream.WriteAsync(_mapper.Map<StudentDTO>(response));
            }
        }

        public override async Task<StudentDTO> GetStudent(ID request, ServerCallContext context)
        {
            var student = await _context.Students.FindAsync(request.Value);
            if (student == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "No such student exists (ID = " + request.Value + ")!"));
            }
            return _mapper.Map<StudentDTO>(student);
        }

        public override async Task GetStudentsWithCourses(QueryParams request, IServerStreamWriter<StudentCoursesDTO> responseStream, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.OrderBy)) request.OrderBy = "studentId";
            if (request.PageNumber == 0) request.PageNumber = 1;
            if (request.PageSize == 0) request.PageSize = 10;
            if (request.PageSize > 100) request.PageSize = 100;
            IOrderedQueryable<Student> students;
            if (string.IsNullOrEmpty(request.FilterBy)) students = _context.Students.Include(s => s.Courses).OrderBy(request.OrderBy);
            else students = _context.Students.Include(s => s.Courses).Where(request.FilterBy).OrderBy(request.OrderBy);
            List<Student> responses = await PaginatedList<Student>.CreateAsync(students, request.PageNumber, request.PageSize);
            foreach (var response in responses)
            {
                await responseStream.WriteAsync(_mapper.Map<StudentCoursesDTO>(response));
            }
        }

        public override async Task<StudentCoursesDTO> GetStudentWithCourses(ID request, ServerCallContext context)
        {
            var student = await _context.Students.Include(s => s.Courses).FirstOrDefaultAsync(s => s.StudentId == request.Value);
            if (student == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "No such student exists (ID = " + request.Value + ")!"));
            }
            return _mapper.Map<StudentCoursesDTO>(student);
        }

        public override async Task<StudentQRCodeDTO> GetStudentQRCode(ID request, ServerCallContext context)
        {
            var student = await _context.Students.FindAsync(request.Value);

            if (student == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "No such student exists (ID = " + request.Value + ")!"));
            }

            return _mapper.Map<StudentQRCodeDTO>(student);
        }

        public override async Task<StudentDTO> AddStudent(StudentDTO request, ServerCallContext context)
        {
            Student studentEntity = _mapper.Map<Student>(request);
            _context.Students.Add(studentEntity);
            await _context.SaveChangesAsync();
            return _mapper.Map<StudentDTO>(studentEntity);
        }

        public override async Task<StudentDTO> ModifyStudent(ChangeStudentDTO request, ServerCallContext context)
        {
            if (request.StudentId != request.Student.StudentId)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    "The specify IDs don't match (StudentId = " + request.StudentId +
                    ", request.Student.StudentId = " + request.Student.StudentId + ")!"));
            }

            _context.Entry(_mapper.Map<Student>(request.Student)).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Students.Any(e => e.StudentId == request.StudentId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "No such student exists (StudentId = " + request.StudentId + ")!"));
                }
                else
                {
                    throw;
                }
            }

            return _mapper.Map<StudentDTO>(await _context.Students.FindAsync(request.StudentId));
        }

        public override async Task<StudentDTO> DeleteStudent(ID request, ServerCallContext context)
        {
            var student = await _context.Students.FindAsync(request.Value);
            if (student == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "No such student exists (StudentId = " + request.Value + ")!"));
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return _mapper.Map<StudentDTO>(student);
        }
    }
}
