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
    public class EmployeeApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public EmployeeApiController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/EmployeeApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            var employees = await _context.Users
                .Include(u => u.Department)
                .Include(u => u.Position)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<EmployeeDto>>(employees));
        }

        // GET: api/EmployeeApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
        {
            var employee = await _context.Users
                .Include(u => u.Department)
                .Include(u => u.Position)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<EmployeeDto>(employee));
        }

        // POST: api/EmployeeApi
        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> PostEmployee(CreateEmployeeDto createEmployeeDto)
        {
            var employee = _mapper.Map<User>(createEmployeeDto);
            employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(createEmployeeDto.Password);

            _context.Users.Add(employee);
            await _context.SaveChangesAsync();

            var employeeDto = _mapper.Map<EmployeeDto>(employee);

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employeeDto);
        }

        // PUT: api/EmployeeApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, UpdateEmployeeDto updateEmployeeDto)
        {
            var employee = await _context.Users.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _mapper.Map(updateEmployeeDto, employee);
            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
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

        // DELETE: api/EmployeeApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Users.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Users.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}