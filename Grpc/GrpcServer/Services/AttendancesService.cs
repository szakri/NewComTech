using GrpcServer.Protos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Common.Data;
using AutoMapper;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Common.Models;

namespace GrpcServer.Services
{
    public class AttendancesService : Attendances.AttendancesBase
    {
        private readonly SchoolContext _context;
        private readonly IMapper _mapper;

        public AttendancesService(SchoolContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public override async Task GetAttendances(QueryParams request, IServerStreamWriter<AttendanceDTO> responseStream, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.OrderBy)) request.OrderBy = "attendanceId";
            if (request.PageNumber == 0) request.PageNumber = 1;
            if (request.PageSize == 0) request.PageSize = 10;
            if (request.PageSize > 100) request.PageSize = 100;
            IOrderedQueryable<Attendance> attendances;
            if (string.IsNullOrEmpty(request.FilterBy))
                attendances = _context.Attendances
                    .Include(a => a.Course)
                    .Include(a => a.Student)
                    .OrderBy(request.OrderBy);
            else
                attendances = _context.Attendances
                    .Include(a => a.Course)
                    .Include(a => a.Student)
                    .Where(request.FilterBy)
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
        }
    }
}
