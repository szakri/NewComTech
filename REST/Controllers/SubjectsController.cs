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
    public class SubjectsController : ControllerBase
    {
        private readonly SchoolContext _context;
        private readonly IMapper _mapper;

        public SubjectsController(SchoolContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Subjects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectDTO>>> GetSubjects(
            [FromQuery] int? pageNumber, [FromQuery] int pageSize = 10, [FromQuery] string orderBy = null)
        {
            if (orderBy == null) orderBy = "subjectId";
            var subjects = _context.Subjects.OrderBy(orderBy);
            return _mapper.Map<List<SubjectDTO>>(await PaginatedList<Subject>.CreateAsync(subjects, pageNumber ?? 1, pageSize));
        }

        // GET: api/Subjects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubjectDTO>> GetSubject(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);

            if (subject == null)
            {
                return NotFound();
            }

            return _mapper.Map<SubjectDTO>(subject);
        }

        // PUT: api/Subjects/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubject(int id, [FromBody] SubjectDTO subject)
        {
            if (id != subject.SubjectId)
            {
                return BadRequest();
            }

            _context.Entry(_mapper.Map<Subject>(subject)).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubjectExists(id))
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

        // POST: api/Subjects
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<SubjectDTO>> PostSubject(SubjectDTO subject)
        {
            if (subject.SubjectId != null) subject.SubjectId = null;
            Subject subjectEntity = _mapper.Map<Subject>(subject);
            _context.Subjects.Add(subjectEntity);
            await _context.SaveChangesAsync();
            subject = _mapper.Map<SubjectDTO>(subjectEntity);
            return CreatedAtAction("GetSubject", new { id = subject.SubjectId }, subject);
        }

        // DELETE: api/Subjects/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<SubjectDTO>> DeleteSubject(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            return _mapper.Map<SubjectDTO>(subject);
        }

        private bool SubjectExists(int id)
        {
            return _context.Subjects.Any(e => e.SubjectId == id);
        }
    }
}
