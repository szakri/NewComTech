using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Common.Models;
using Common.Data;
using AutoMapper;
using System.Linq.Dynamic.Core;

namespace REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly SchoolContext _context;
        private readonly IMapper _mapper;

        public StudentsController(SchoolContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetStudents(
            [FromQuery] int? pageNumber, [FromQuery] int pageSize = 10, [FromQuery] string orderBy = null)
        {
            if (orderBy == null) orderBy = "studentId";
            var students = _context.Students.OrderBy(orderBy);
            return _mapper.Map<List<StudentDTO>>(await PaginatedList<Student>.CreateAsync(students, pageNumber ?? 1, pageSize));
        }

        // GET: api/Students/withCourses
        [HttpGet("withCourses")]
        public async Task<ActionResult<IEnumerable<StudentCoursesDTO>>> GetStudentsWithCourses(
            [FromQuery] int? pageNumber, [FromQuery] int pageSize = 10, [FromQuery] string orderBy = null)
        {
            if (orderBy == null) orderBy = "studentId";
            var students = _context.Students.Include(s => s.Courses).OrderBy(orderBy);
            return _mapper.Map<List<StudentCoursesDTO>>(await PaginatedList<Student>.CreateAsync(students, pageNumber ?? 1, pageSize));
        }

        // GET: api/Students/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDTO>> GetStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return _mapper.Map<StudentDTO>(student);
        }

        // GET: api/Students/5/withCourses
        [HttpGet("{id}/withCourses")]
        public async Task<ActionResult<StudentCoursesDTO>> GetStudentWithCourses(int id)
        {
            var student = await _context.Students.Include(s => s.Courses).FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                return NotFound();
            }

            return _mapper.Map<StudentCoursesDTO>(student);
        }

        // GET: api/Students/5/QR
        [HttpGet("{id}/QR")]
        public async Task<ActionResult<StudentQRCodeDTO>> GetStudentQRCode(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return _mapper.Map<StudentQRCodeDTO>(student);
        }

        // PUT: api/Students/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, [FromBody] StudentDTO student)
        {
            if (id != student.StudentId)
            {
                return BadRequest();
            }

            _context.Entry(_mapper.Map<Student>(student)).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
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

        // POST: api/Students
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<StudentDTO>> PostStudent([FromBody] StudentDTO student)
        {
            Student studentEntity = _mapper.Map<Student>(student);
            _context.Students.Add(studentEntity);
            await _context.SaveChangesAsync();
            student = _mapper.Map<StudentDTO>(studentEntity);
            return CreatedAtAction("GetStudent", new { id = student.StudentId }, student);
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<StudentDTO>> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return _mapper.Map<StudentDTO>(student);
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentId == id);
        }
    }
}
