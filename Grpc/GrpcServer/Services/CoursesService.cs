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
    public class CoursesService : Courses.CoursesBase
    {
        private readonly SchoolContext _context;
        private readonly IMapper _mapper;

        public CoursesService(SchoolContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public override async Task GetCourses(QueryParams request, IServerStreamWriter<CourseDTO> responseStream, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.OrderBy)) request.OrderBy = "courseId";
            if (request.PageNumber == 0) request.PageNumber = 1;
            if (request.PageSize == 0) request.PageSize = 10;
            if (request.PageSize > 100) request.PageSize = 100;
            IOrderedQueryable<Course> courses;
            if (string.IsNullOrEmpty(request.FilterBy)) courses = _context.Courses.OrderBy(request.OrderBy);
            else courses = _context.Courses.Where(request.FilterBy).OrderBy(request.OrderBy);
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
            if (string.IsNullOrEmpty(request.OrderBy)) request.OrderBy = "courseId";
            if (request.PageNumber == 0) request.PageNumber = 1;
            if (request.PageSize == 0) request.PageSize = 10;
            IOrderedQueryable<Course> courses;
            if (string.IsNullOrEmpty(request.FilterBy)) courses = _context.Courses.Include(c => c.Subject).OrderBy(request.OrderBy);
            else courses = _context.Courses.Include(c => c.Subject).Where(request.FilterBy).OrderBy(request.OrderBy);
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
    }
}
