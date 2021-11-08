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
    public class CoursesController : ODataController
    {
        private readonly SchoolContext _context;

        public CoursesController(SchoolContext context)
        {
            _context = context;
        }

        [HttpGet]
        [EnableQuery(PageSize = 10)]
        public IQueryable<Course> GetCourses() =>_context.Courses;

        [HttpGet]
        [ODataRoute("courses({id})")]
        public async Task<ActionResult<Course>> GetCourse([FromODataUri] int id)
        {
            var course = await _context.Courses
                .Include(c => c.Subject)
                .Include(c => c.Students)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            return course;
        }

        [HttpPost]
        public async Task<IActionResult> PostCourse([FromBody] Course course)
        {
            if (_context.Courses.Any(c => c.CourseId == course.CourseId))
                return BadRequest($"An attendance already exists with this id: {course.CourseId}!");

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return Created(course);
        }

        [HttpPut]
        [ODataRoute("courses({id})")]
        public async Task<IActionResult> PutCourse([FromODataUri] int id, [FromBody] Course course)
        {
            if (id != course.CourseId)
            {
                return BadRequest();
            }

            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Courses.Any(e => e.CourseId == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(course);
        }

        [HttpDelete]
        [ODataRoute("courses({id})")]
        public async Task<ActionResult<Course>> DeleteCourse([FromODataUri] int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return course;
        }
    }
}
