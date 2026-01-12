using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRManagementSystem.Web.Data;
using HRManagementSystem.Web.Models;

using AutoMapper;
using HRManagementSystem.Web.Models.Dtos;

namespace HRManagementSystem.Web.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DepartmentApiController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/DepartmentApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartments()
        {
            var departments = await _context.Departments
                .Include(d => d.Manager)
                .Include(d => d.Children)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<DepartmentDto>>(departments));
        }

        // GET: api/DepartmentApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentDto>> GetDepartment(int id)
        {
            var department = await _context.Departments
                .Include(d => d.Manager)
                .Include(d => d.Children)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<DepartmentDto>(department));
        }

        // POST: api/DepartmentApi
        [HttpPost]
        public async Task<ActionResult<DepartmentDto>> PostDepartment(CreateDepartmentDto createDepartmentDto)
        {
            var department = _mapper.Map<Department>(createDepartmentDto);
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            var departmentDto = _mapper.Map<DepartmentDto>(department);

            return CreatedAtAction(nameof(GetDepartment), new { id = department.Id }, departmentDto);
        }

        // PUT: api/DepartmentApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(int id, UpdateDepartmentDto updateDepartmentDto)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            _mapper.Map(updateDepartmentDto, department);
            _context.Entry(department).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
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

        // DELETE: api/DepartmentApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}