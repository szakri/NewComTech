using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Common.Data;
using Common.Models;
using System.Linq.Dynamic.Core;
using AutoMapper;

namespace REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendancesController : ControllerBase
    {
        private readonly SchoolContext _context;
        private readonly IMapper _mapper;

        public AttendancesController(SchoolContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Attendances
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AttendanceDTO>>> GetAttendances(
            [FromQuery] int? pageNumber, [FromQuery] int pageSize = 10, [FromQuery] string orderBy = null, [FromQuery] string filterBy = null)
        {
            if (orderBy == null) orderBy = "attendanceId";
            IOrderedQueryable<Attendance> attendances;
            if (string.IsNullOrEmpty(filterBy))
                attendances = _context.Attendances
                    .Include(a => a.Course)
                    .Include(a => a.Student)
                    .OrderBy(orderBy);
            else
                attendances = _context.Attendances
                    .Include(a => a.Course)
                    .Include(a => a.Student)
                    .Where(filterBy)
                    .OrderBy(orderBy);
            return _mapper.Map<List<AttendanceDTO>>(await PaginatedList<Attendance>.CreateAsync(attendances, pageNumber ?? 1, pageSize));
        }

        // GET: api/Attendances/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AttendanceDTO>> GetAttendance(int id)
        {
            var attendance = await _context.Attendances
                .Include(a => a.Course)
                .Include(a => a.Student)
                .FirstOrDefaultAsync(a => a.AttendanceId == id);

            if (attendance == null)
            {
                return NotFound();
            }

            return _mapper.Map<AttendanceDTO>(attendance);
        }

        // PUT: api/Attendances/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAttendance(int id, AttendanceDTO attendance)
        {
            if (id != attendance.AttendanceId)
            {
                return BadRequest();
            }

            _context.Entry(_mapper.Map<Attendance>(attendance)).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttendanceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Attendances
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AttendanceDTO>> PostAttendance(AttendanceDTO attendance)
        {
            if (attendance.AttendanceId != null) attendance.AttendanceId = null;
            Attendance attendanceEntity = _mapper.Map<Attendance>(attendance);
            _context.Attendances.Add(attendanceEntity);
            await _context.SaveChangesAsync();
            attendance = _mapper.Map<AttendanceDTO>(attendanceEntity);
            return CreatedAtAction("GetAttendance", new { id = attendance.AttendanceId }, attendance);
        }

        // DELETE: api/Attendances/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }

            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AttendanceExists(int id)
        {
            return _context.Attendances.Any(e => e.AttendanceId == id);
        }
    }
}
