using AutoMapper;
using HRManagementSystem.Web.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRManagementSystem.Web.Data;
using HRManagementSystem.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRManagementSystem.Web.Controllers.Api
{
    [ApiController]
    [Route("api/roles")]
    public class RoleApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public RoleApiController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            var roles = await _context.Roles.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<RoleDto>>(roles));
        }

        // GET: api/roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<RoleDto>(role));
        }

        // POST: api/roles
        [HttpPost]
        public async Task<ActionResult<RoleDto>> PostRole(CreateRoleDto createRoleDto)
        {
            var role = _mapper.Map<Role>(createRoleDto);

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            var roleDto = _mapper.Map<RoleDto>(role);

            return CreatedAtAction(nameof(GetRole), new { id = roleDto.Id }, roleDto);
        }

        // PUT: api/roles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(int id, UpdateRoleDto updateRoleDto)
        {
            var roleFromDb = await _context.Roles.FindAsync(id);

            if (roleFromDb == null)
            {
                return NotFound();
            }

            _mapper.Map(updateRoleDto, roleFromDb);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(id))
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

        // DELETE: api/roles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }
    }
}