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
    public class PermissionApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PermissionApiController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/PermissionApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetPermissions()
        {
            var permissions = await _context.Permissions
                .Include(p => p.RolePermissions)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<PermissionDto>>(permissions));
        }

        // GET: api/PermissionApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PermissionDto>> GetPermission(int id)
        {
            var permission = await _context.Permissions
                .Include(p => p.RolePermissions)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (permission == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PermissionDto>(permission));
        }

        // POST: api/PermissionApi
        [HttpPost]
        public async Task<ActionResult<PermissionDto>> PostPermission(CreatePermissionDto createPermissionDto)
        {
            var permission = _mapper.Map<Permission>(createPermissionDto);
            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();

            var permissionDto = _mapper.Map<PermissionDto>(permission);
            return CreatedAtAction(nameof(GetPermission), new { id = permission.Id }, permissionDto);
        }

        // PUT: api/PermissionApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPermission(int id, UpdatePermissionDto updatePermissionDto)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null)
            {
                return NotFound();
            }

            _mapper.Map(updatePermissionDto, permission);
            _context.Entry(permission).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PermissionExists(id))
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

        // DELETE: api/PermissionApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null)
            {
                return NotFound();
            }

            _context.Permissions.Remove(permission);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PermissionExists(int id)
        {
            return _context.Permissions.Any(e => e.Id == id);
        }
    }
}
