using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRManagementSystem.Web.Data;
using HRManagementSystem.Web.Models;
using HRManagementSystem.Web.Models.Dtos;
using AutoMapper;

namespace HRManagementSystem.Web.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorklogApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public WorklogApiController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/WorklogApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorklogDto>>> GetWorklogs()
        {
            var worklogs = await _context.Worklogs
                .Include(w => w.Employee)
                .Include(w => w.Project)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<WorklogDto>>(worklogs));
        }

        // GET: api/WorklogApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WorklogDto>> GetWorklog(int id)
        {
            var worklog = await _context.Worklogs
                .Include(w => w.Employee)
                .Include(w => w.Project)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (worklog == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<WorklogDto>(worklog));
        }

        // POST: api/WorklogApi
        [HttpPost]
        public async Task<ActionResult<WorklogDto>> PostWorklog(CreateWorklogDto createWorklogDto)
        {
            var worklog = _mapper.Map<Worklog>(createWorklogDto);
            _context.Worklogs.Add(worklog);
            await _context.SaveChangesAsync();

            var worklogDto = _mapper.Map<WorklogDto>(worklog);
            return CreatedAtAction(nameof(GetWorklog), new { id = worklog.Id }, worklogDto);
        }

        // PUT: api/WorklogApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorklog(int id, UpdateWorklogDto updateWorklogDto)
        {
            var worklog = await _context.Worklogs.FindAsync(id);
            if (worklog == null)
            {
                return NotFound();
            }

            _mapper.Map(updateWorklogDto, worklog);
            _context.Entry(worklog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorklogExists(id))
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

        // DELETE: api/WorklogApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorklog(int id)
        {
            var worklog = await _context.Worklogs.FindAsync(id);
            if (worklog == null)
            {
                return NotFound();
            }

            _context.Worklogs.Remove(worklog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WorklogExists(int id)
        {
            return _context.Worklogs.Any(e => e.Id == id);
        }
    }
}
