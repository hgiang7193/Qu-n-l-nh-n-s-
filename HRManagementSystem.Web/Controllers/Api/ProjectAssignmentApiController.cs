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
    public class ProjectAssignmentApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProjectAssignmentApiController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/ProjectAssignmentApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectAssignmentDto>>> GetProjectAssignments()
        {
            var projectAssignments = await _context.ProjectAssignments
                .Include(pa => pa.Project)
                .Include(pa => pa.Employee)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<ProjectAssignmentDto>>(projectAssignments));
        }

        // GET: api/ProjectAssignmentApi/my-assignments
        [HttpGet("my-assignments")]
        public async Task<ActionResult<IEnumerable<ProjectAssignmentDto>>> GetMyProjectAssignments()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var projectAssignments = await _context.ProjectAssignments
                .Include(pa => pa.Project)
                .Include(pa => pa.Employee)
                .Where(pa => pa.EmployeeId == userId && pa.Status == "active")
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<ProjectAssignmentDto>>(projectAssignments));
        }

        // GET: api/ProjectAssignmentApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectAssignmentDto>> GetProjectAssignment(int id)
        {
            var projectAssignment = await _context.ProjectAssignments
                .Include(pa => pa.Project)
                .Include(pa => pa.Employee)
                .FirstOrDefaultAsync(pa => pa.Id == id);

            if (projectAssignment == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ProjectAssignmentDto>(projectAssignment));
        }

        // POST: api/ProjectAssignmentApi
        [HttpPost]
        public async Task<ActionResult<ProjectAssignmentDto>> PostProjectAssignment(CreateProjectAssignmentDto createProjectAssignmentDto)
        {
            var projectAssignment = _mapper.Map<ProjectAssignment>(createProjectAssignmentDto);
            _context.ProjectAssignments.Add(projectAssignment);
            await _context.SaveChangesAsync();

            var projectAssignmentDto = _mapper.Map<ProjectAssignmentDto>(projectAssignment);
            return CreatedAtAction(nameof(GetProjectAssignment), new { id = projectAssignment.Id }, projectAssignmentDto);
        }

        // PUT: api/ProjectAssignmentApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProjectAssignment(int id, UpdateProjectAssignmentDto updateProjectAssignmentDto)
        {
            var projectAssignment = await _context.ProjectAssignments.FindAsync(id);
            if (projectAssignment == null)
            {
                return NotFound();
            }

            _mapper.Map(updateProjectAssignmentDto, projectAssignment);
            _context.Entry(projectAssignment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectAssignmentExists(id))
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

        // DELETE: api/ProjectAssignmentApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjectAssignment(int id)
        {
            var projectAssignment = await _context.ProjectAssignments.FindAsync(id);
            if (projectAssignment == null)
            {
                return NotFound();
            }

            _context.ProjectAssignments.Remove(projectAssignment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectAssignmentExists(int id)
        {
            return _context.ProjectAssignments.Any(e => e.Id == id);
        }
    }
}
