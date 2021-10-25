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
    public class SubjectsController : ODataController
    {
        private readonly SchoolContext _context;

        public SubjectsController(SchoolContext context)
        {
            _context = context;
        }

        [HttpGet]
        [EnableQuery(PageSize = 10)]
        public async Task<ActionResult<IEnumerable<Subject>>> GetSubjects()
        {
            return await _context.Subjects.ToListAsync();
        }

        [HttpGet]
        [ODataRoute("subjects({id})")]
        public async Task<ActionResult<Subject>> GetSubject([FromODataUri] int id)
        {
            var subject = await _context.Subjects.FindAsync(id);

            if (subject == null)
            {
                return NotFound();
            }

            return subject;
        }

        [HttpPost]
        public async Task<IActionResult> PostSubject([FromBody] Subject subject)
        {
            if (_context.Subjects.Any(s => s.SubjectId == subject.SubjectId))
                return BadRequest($"An attendance already exists with this id: {subject.SubjectId}!");

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return Created(subject);
        }

        [HttpPut]
        [ODataRoute("subjects({id})")]
        public async Task<IActionResult> PutSubject([FromODataUri] int id, [FromBody] Subject subject)
        {
            if (id != subject.SubjectId)
            {
                return BadRequest();
            }
            _context.Entry(subject).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Subjects.Any(e => e.SubjectId == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(subject);
        }

        [HttpDelete]
        [ODataRoute("subjects({id})")]
        public async Task<ActionResult<Subject>> DeleteSubject([FromODataUri] int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            return subject;
        }
    }
}
