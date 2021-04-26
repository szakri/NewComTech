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
using System.Linq.Dynamic.Core;

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

        /*public override async Task GetStudents(QueryParams request, IServerStreamWriter<StudentDTO> responseStream, ServerCallContext context)
        {
            if (request.OrderBy == "") request.OrderBy = "studentId";
            if (request.PageNumber == 0) request.PageNumber = 1;
            if (request.PageSize == 0) request.PageSize = 10;
            var students = _context.Students.OrderBy(request.OrderBy);
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
            if (request.OrderBy == "") request.OrderBy = "studentId";
            if (request.PageNumber == 0) request.PageNumber = 1;
            if (request.PageSize == 0) request.PageSize = 10;
            var students = _context.Students.Include(s => s.Courses).OrderBy(request.OrderBy);
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

        public override async Task GetCourses(QueryParams request, IServerStreamWriter<CourseDTO> responseStream, ServerCallContext context)
        {
            if (request.OrderBy == "") request.OrderBy = "courseId";
            if (request.PageNumber == 0) request.PageNumber = 1;
            if (request.PageSize == 0) request.PageSize = 10;
            var courses = _context.Courses.OrderBy(request.OrderBy);
            List<Course> responses = await PaginatedList<Course>.CreateAsync(courses, request.PageNumber, request.PageSize);
            foreach (var response in responses)
            {
                await responseStream.WriteAsync(_mapper.Map<CourseDTO>(response));
            }
        }

        public override async Task<CourseDTO> GetCourse(ID request, ServerCallContext context)
        {
            var course = await _context.Courses.FindAsync(request.Value);
            if (course == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "No such course exists (ID = " + request.Value + ")!"));
            }
            return _mapper.Map<CourseDTO>(course);
        }

        public override async Task GetCoursesWithSubject(QueryParams request, IServerStreamWriter<CourseSubjectDTO> responseStream, ServerCallContext context)
        {
            if (request.OrderBy == "") request.OrderBy = "courseId";
            if (request.PageNumber == 0) request.PageNumber = 1;
            if (request.PageSize == 0) request.PageSize = 10;
            var courses = _context.Courses.Include(c => c.Subject).OrderBy(request.OrderBy);
            List<Course> responses = await PaginatedList<Course>.CreateAsync(courses, request.PageNumber, request.PageSize);
            foreach (var response in responses)
            {
                await responseStream.WriteAsync(_mapper.Map<CourseSubjectDTO>(response));
            }
        }

        public override async Task<CourseSubjectDTO> GetCourseWithSubject(ID request, ServerCallContext context)
        {
            var course = await _context.Courses.Include(c => c.Subject).FirstOrDefaultAsync(c => c.CourseId == request.Value);
            if (course == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "No such course exists (ID = " + request.Value + ")!"));
            }
            return _mapper.Map<CourseSubjectDTO>(course);
        }

        public override async Task<CourseDTO> AddCourse(CourseDTO request, ServerCallContext context)
        {
            Course courseEntity = _mapper.Map<Course>(request);
            _context.Courses.Add(courseEntity);
            await _context.SaveChangesAsync();
            return _mapper.Map<CourseDTO>(courseEntity);
        }

        public override async Task<CourseDTO> ModifyCourse(ChangeCourseDTO request, ServerCallContext context)
        {
            if (request.CourseId != request.Course.CourseId)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    "The specify IDs don't match (CourseId = " + request.CourseId +
                    ", request.Student.StudentId = " + request.Course.CourseId + ")!"));
            }

            _context.Entry(_mapper.Map<Course>(request.Course)).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Courses.Any(e => e.CourseId == request.CourseId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "No such course exists (CourseId = " + request.CourseId + ")!"));
                }
                else
                {
                    throw;
                }
            }

            return _mapper.Map<CourseDTO>(await _context.Courses.FindAsync(request.CourseId));
        }

        public override async Task<CourseDTO> DeleteCourse(ID request, ServerCallContext context)
        {
            var course = await _context.Courses.FindAsync(request.Value);
            if (course == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "No such course exists (CourseId = " + request.Value + ")!"));
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return _mapper.Map<CourseDTO>(course);
        }

        public override async Task GetSubjects(QueryParams request, IServerStreamWriter<SubjectDTO> responseStream, ServerCallContext context)
        {
            if (request.OrderBy == "") request.OrderBy = "subjectId";
            if (request.PageNumber == 0) request.PageNumber = 1;
            if (request.PageSize == 0) request.PageSize = 10;
            var subjects = _context.Subjects.OrderBy(request.OrderBy);
            List<Subject> responses = await PaginatedList<Subject>.CreateAsync(subjects, request.PageNumber, request.PageSize);
            foreach (var response in responses)
            {
                await responseStream.WriteAsync(_mapper.Map<SubjectDTO>(response));
            }
        }

        public override async Task<SubjectDTO> GetSubject(ID request, ServerCallContext context)
        {
            var subject = await _context.Subjects.FindAsync(request.Value);
            if (subject == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "No such subject exists (ID = " + request.Value + ")!"));
            }
            return _mapper.Map<SubjectDTO>(subject);
        }

        public override async Task<SubjectDTO> AddSubject(SubjectDTO request, ServerCallContext context)
        {
            Subject subjectEntity = _mapper.Map<Subject>(request);
            _context.Subjects.Add(subjectEntity);
            await _context.SaveChangesAsync();
            return _mapper.Map<SubjectDTO>(subjectEntity);
        }

        public override async Task<SubjectDTO> ModifySubject(ChangeSubjectDTO request, ServerCallContext context)
        {
            if (request.SubjectId != request.Subject.SubjectId)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    "The specify IDs don't match (StudentId = " + request.SubjectId +
                    ", request.Student.StudentId = " + request.Subject.SubjectId + ")!"));
            }

            _context.Entry(_mapper.Map<Subject>(request.Subject)).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Subjects.Any(e => e.SubjectId == request.SubjectId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "No such course exists (SubjectId = " + request.SubjectId + ")!"));
                }
                else
                {
                    throw;
                }
            }

            return _mapper.Map<SubjectDTO>(await _context.Subjects.FindAsync(request.SubjectId));
        }

        public override async Task<SubjectDTO> DeleteSubject(ID request, ServerCallContext context)
        {
            var subject = await _context.Subjects.FindAsync(request.Value);
            if (subject == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "No such course exists (SubjectId = " + request.Value + ")!"));
            }

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            return _mapper.Map<SubjectDTO>(subject);
        }

        public override async Task GetAttendances(QueryParams request, IServerStreamWriter<AttendanceDTO> responseStream, ServerCallContext context)
        {
            if (request.OrderBy == "") request.OrderBy = "attendanceId";
            if (request.PageNumber == 0) request.PageNumber = 1;
            if (request.PageSize == 0) request.PageSize = 10;
            var attendances = _context.Attendances
                .Include(a => a.Course)
                .Include(a => a.Student)
                .OrderBy(request.OrderBy);
            List<Attendance> responses = await PaginatedList<Attendance>.CreateAsync(attendances, request.PageNumber, request.PageSize);
            foreach (var response in responses)
            {
                await responseStream.WriteAsync(_mapper.Map<AttendanceDTO>(response));
            }
        }

        public override async Task<AttendanceDTO> GetAttendance(ID request, ServerCallContext context)
        {
            var attendance = await _context.Attendances
                .Include(a => a.Course)
                .Include(a => a.Student)
                .FirstOrDefaultAsync(a => a.AttendanceId == request.Value);

            if (attendance == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "No such attendance exists (ID = " + request.Value + ")!"));
            }

            return _mapper.Map<AttendanceDTO>(attendance);
        }

        public override async Task<AttendanceDTO> AddAttendance(AttendanceDTO request, ServerCallContext context)
        {
            Attendance attendanceEntity = _mapper.Map<Attendance>(request);
            _context.Attendances.Add(attendanceEntity);
            await _context.SaveChangesAsync();
            return _mapper.Map<AttendanceDTO>(attendanceEntity);
        }

        public override async Task<AttendanceDTO> ModifyAttendance(ChangeAttendanceDTO request, ServerCallContext context)
        {
            if (request.AttendanceId != request.Attendance.AttendanceId)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    "The specify IDs don't match (StudentId = " + request.AttendanceId +
                    ", request.Student.StudentId = " + request.Attendance.AttendanceId + ")!"));
            }

            _context.Entry(_mapper.Map<Attendance>(request.Attendance)).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Subjects.Any(e => e.SubjectId == request.AttendanceId))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "No such course exists (AttendanceId = " + request.AttendanceId + ")!"));
                }
                else
                {
                    throw;
                }
            }

            return _mapper.Map<AttendanceDTO>(await _context.Attendances.FindAsync(request.AttendanceId));
        }

        public override async Task<AttendanceDTO> DeleteAttendance(ID request, ServerCallContext context)
        {
            var attendance = await _context.Attendances.FindAsync(request.Value);
            if (attendance == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "No such course exists (AttendanceId = " + request.Value + ")!"));
            }

            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync();

            return _mapper.Map<AttendanceDTO>(attendance);
        }*/
    }
}
