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
    public class UserRoleApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserRoleApiController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/UserRoleApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserRoleDto>>> GetUserRoles()
        {
            var userRoles = await _context.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<UserRoleDto>>(userRoles));
        }

        // GET: api/UserRoleApi/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<UserRoleDto>>> GetUserRolesByUser(int userId)
        {
            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .ToListAsync();

            if (!userRoles.Any())
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IEnumerable<UserRoleDto>>(userRoles));
        }

        // POST: api/UserRoleApi
        [HttpPost]
        public async Task<ActionResult<UserRoleDto>> PostUserRole(CreateUserRoleDto createUserRoleDto)
        {
            // Check if UserRole already exists
            if (await _context.UserRoles.AnyAsync(ur => ur.UserId == createUserRoleDto.UserId && ur.RoleId == createUserRoleDto.RoleId))
            {
                return BadRequest("This user role assignment already exists.");
            }

            var userRole = _mapper.Map<UserRole>(createUserRoleDto);
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            var userRoleDto = _mapper.Map<UserRoleDto>(userRole);
            return CreatedAtAction(nameof(GetUserRoles), userRoleDto);
        }

        // DELETE: api/UserRoleApi/user/5/role/3
        [HttpDelete("user/{userId}/role/{roleId}")]
        public async Task<IActionResult> DeleteUserRole(int userId, int roleId)
        {
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

            if (userRole == null)
            {
                return NotFound();
            }

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
