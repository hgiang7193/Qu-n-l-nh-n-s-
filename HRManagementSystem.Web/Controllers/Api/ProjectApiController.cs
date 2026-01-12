using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRManagementSystem.Web.Models;
using HRManagementSystem.Web.Data;

using AutoMapper;
using AutoMapper.QueryableExtensions;
using HRManagementSystem.Web.Models.Dtos;

namespace HRManagementSystem.Web.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProjectApiController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Project
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        {
            return await _context.Projects
                .ProjectTo<ProjectDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // GET: api/Project/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProject(int id)
        {
            var project = await _context.Projects
                .ProjectTo<ProjectDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        // POST: api/Project
        [HttpPost]
        public async Task<ActionResult<ProjectDto>> PostProject(CreateProjectDto createProjectDto)
        {
            var project = _mapper.Map<Project>(createProjectDto);

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            var projectDto = _mapper.Map<ProjectDto>(project);

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, projectDto);
        }

        // PUT: api/Project/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, UpdateProjectDto updateProjectDto)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            _mapper.Map(updateProjectDto, project);
            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
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

        // DELETE: api/Project/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}