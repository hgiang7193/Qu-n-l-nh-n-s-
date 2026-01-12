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
    public class RolePermissionApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public RolePermissionApiController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/RolePermissionApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolePermissionDto>>> GetRolePermissions()
        {
            var rolePermissions = await _context.RolePermissions
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<RolePermissionDto>>(rolePermissions));
        }

        // GET: api/RolePermissionApi/role/5
        [HttpGet("role/{roleId}")]
        public async Task<ActionResult<IEnumerable<RolePermissionDto>>> GetRolePermissionsByRole(int roleId)
        {
            var rolePermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .ToListAsync();

            if (!rolePermissions.Any())
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IEnumerable<RolePermissionDto>>(rolePermissions));
        }

        // POST: api/RolePermissionApi
        [HttpPost]
        public async Task<ActionResult<RolePermissionDto>> PostRolePermission(CreateRolePermissionDto createRolePermissionDto)
        {
            // Check if RolePermission already exists
            if (await _context.RolePermissions.AnyAsync(rp => rp.RoleId == createRolePermissionDto.RoleId && rp.PermissionId == createRolePermissionDto.PermissionId))
            {
                return BadRequest("This role permission assignment already exists.");
            }

            var rolePermission = _mapper.Map<RolePermission>(createRolePermissionDto);
            _context.RolePermissions.Add(rolePermission);
            await _context.SaveChangesAsync();

            var rolePermissionDto = _mapper.Map<RolePermissionDto>(rolePermission);
            return CreatedAtAction(nameof(GetRolePermissions), rolePermissionDto);
        }

        // DELETE: api/RolePermissionApi/role/5/permission/3
        [HttpDelete("role/{roleId}/permission/{permissionId}")]
        public async Task<IActionResult> DeleteRolePermission(int roleId, int permissionId)
        {
            var rolePermission = await _context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

            if (rolePermission == null)
            {
                return NotFound();
            }

            _context.RolePermissions.Remove(rolePermission);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
