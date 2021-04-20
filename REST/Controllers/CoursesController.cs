using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Common.Data;
using Common.Models;
using AutoMapper;
using System.Linq.Dynamic.Core;

namespace REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly SchoolContext _context;

        private readonly IMapper _mapper;

        public CoursesController(SchoolContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDTO>>> GetCourses(
            [FromQuery] int? pageNumber, [FromQuery] int pageSize = 10, [FromQuery] string orderBy = null)
        {
            if (orderBy == null) orderBy = "courseId";
            var courses = _context.Courses.OrderBy(orderBy);
            return _mapper.Map<List<CourseDTO>>(await PaginatedList<Course>.CreateAsync(courses, pageNumber ?? 1, pageSize));
        }

        // GET: api/Courses/withSubject
        [HttpGet("withSubject")]
        public async Task<ActionResult<IEnumerable<CourseSubjectDTO>>> GetCoursesWithSubject(
            [FromQuery] int? pageNumber, [FromQuery] int pageSize = 10, [FromQuery] string orderBy = null)
        {
            if (orderBy == null) orderBy = "courseId";
            var courses = _context.Courses.Include(c => c.Subject).OrderBy(orderBy);
            return _mapper.Map<List<CourseSubjectDTO>>(await PaginatedList<Course>.CreateAsync(courses, pageNumber ?? 1, pageSize));
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDTO>> GetCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            return _mapper.Map<CourseDTO>(course);
        }

        // GET: api/Courses/5/withSubject
        [HttpGet("{id}/withSubject")]
        public async Task<ActionResult<CourseSubjectDTO>> GetCoursesWithSubject(int id)
        {
            var course = await _context.Courses.Include(c => c.Subject).FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            return _mapper.Map<CourseSubjectDTO>(course);
        }

        // PUT: api/Courses/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(int id, [FromBody] CourseDTO course)
        {
            if (id != course.CourseId)
            {
                return BadRequest();
            }

            _context.Entry(_mapper.Map<Student>(course)).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
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

        // POST: api/Courses
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<CourseDTO>> PostCourse([FromBody] CourseDTO course)
        {
            Course courseEntity = _mapper.Map<Course>(course);
            _context.Courses.Add(courseEntity);
            await _context.SaveChangesAsync();
            course = _mapper.Map<CourseDTO>(courseEntity);
            return CreatedAtAction("GetCourse", new { id = course.CourseId }, course);
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CourseDTO>> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return _mapper.Map<CourseDTO>(course);
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.CourseId == id);
        }
    }
}
