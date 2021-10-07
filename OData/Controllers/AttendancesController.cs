using Common.Data;
using Common.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Results;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OData.Controllers
{
    public class AttendancesController : ODataController
    {
        private readonly SchoolContext _context;

        public AttendancesController(SchoolContext context)
        {
            _context = context;
        }

        [HttpGet]
        [EnableQuery(PageSize = 10)]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetAttendances()
        {
            return await _context.Attendances.Include(a => a.Course).Include(a => a.Student).ToListAsync();
        }

        [HttpGet]
        [ODataRoute("attendances({id})")]
        public async Task<ActionResult<Attendance>> GetAttendance([FromODataUri] int id)
        {
            var attendance = await _context.Attendances
                .Include(a => a.Course)
                .Include(a => a.Student)
                .FirstOrDefaultAsync(a => a.AttendanceId == id);

            if (attendance == null)
            {
                return NotFound();
            }

            return attendance;
        }

        [HttpPost]
        public async Task<IActionResult> PostAttendance([FromBody] Attendance attendance)
        {
            if (_context.Attendances.Any(a => a.AttendanceId == attendance.AttendanceId))
                return BadRequest($"An attendance already exists with this id: {attendance.AttendanceId}!");

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return Created(attendance);
        }

        [HttpPut]
        [ODataRoute("attendances({id})")]
        public async Task<IActionResult> PutAttendance([FromODataUri] int id, [FromBody] Attendance attendance)
        {
            if (id != attendance.AttendanceId)
            {
                return BadRequest();
            }
            _context.Entry(attendance).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Attendances.Any(e => e.AttendanceId == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(attendance);
        }

        [HttpDelete]
        [ODataRoute("attendances({id})")]
        public async Task<ActionResult<Attendance>> DeleteAttendance([FromODataUri] int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }

            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync();

            return attendance;
        }
    }
}
