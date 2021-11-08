using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Common.Data;
using Common.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;

namespace OData.Controllers
{
    public class StudentsController : ODataController
    {
        private readonly SchoolContext _context;

        public StudentsController(SchoolContext context)
        {
            _context = context;
        }

        [HttpGet]
        [EnableQuery]
        public IQueryable<Student> GetStudents() => _context.Students;

        [HttpGet]
        [EnableQuery]
        [ODataRoute("students({id})")]
        public async Task<ActionResult<Student>> GetStudent([FromODataUri] int id)
        {
            _context.Students.Include(s => s.Courses).ThenInclude(c => c.Subject).FirstOrDefault(s => s.StudentId == id);
            var student = await _context.Students
                .Include(s => s.Courses)
                .ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }
        

        /*[HttpGet]
        public async Task<FileContentResult> GetQRCode(int studentId)
        {
            var student = await _context.Students.FindAsync(studentId);

            if (student == null)
            {
                return null;
            }

            return File(student.QRCode, "application/png");
        }*/

        [HttpGet]
        public async Task<byte[]> GetQRCode(int studentId)
        {
            var student = await _context.Students.FindAsync(studentId);

            if (student == null)
            {
                return null;
            }

            return student.QRCode;
        }

        [HttpPost]
        public async Task<IActionResult> PostStudent([FromBody] Student student)
        {
            if (_context.Students.Any(s => s.StudentId == student.StudentId))
                return BadRequest($"A student already exists with this id: {student.StudentId}!");

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return Created(student);
        }

        [HttpPut]
        [ODataRoute("students({id})")]
        public async Task<IActionResult> PutStudent([FromODataUri] int id, [FromBody] Student student)
        {
            if (id != student.StudentId)
            {
                return BadRequest();
            }
            _context.Entry(student).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_context.Students.Any(e => e.StudentId == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(student);
        }

        [HttpDelete]
        [ODataRoute("students({id})")]
        public async Task<ActionResult<Student>> DeleteStudent([FromODataUri] int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return student;
        }
    }
}
