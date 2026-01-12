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
    public class LeaveRequestApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public LeaveRequestApiController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/LeaveRequestApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> GetLeaveRequests()
        {
            var leaveRequests = await _context.LeaveRequests
                .Include(l => l.Employee)
                .Include(l => l.ApprovedByUser)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<LeaveRequestDto>>(leaveRequests));
        }

        // GET: api/LeaveRequestApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveRequestDto>> GetLeaveRequest(int id)
        {
            var leaveRequest = await _context.LeaveRequests
                .Include(l => l.Employee)
                .Include(l => l.ApprovedByUser)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leaveRequest == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<LeaveRequestDto>(leaveRequest));
        }

        // POST: api/LeaveRequestApi
        [HttpPost]
        public async Task<ActionResult<LeaveRequestDto>> PostLeaveRequest(CreateLeaveRequestDto createLeaveRequestDto)
        {
            var leaveRequest = _mapper.Map<LeaveRequest>(createLeaveRequestDto);
            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();

            var leaveRequestDto = _mapper.Map<LeaveRequestDto>(leaveRequest);
            return CreatedAtAction(nameof(GetLeaveRequest), new { id = leaveRequest.Id }, leaveRequestDto);
        }

        // PUT: api/LeaveRequestApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLeaveRequest(int id, UpdateLeaveRequestDto updateLeaveRequestDto)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null)
            {
                return NotFound();
            }

            _mapper.Map(updateLeaveRequestDto, leaveRequest);
            _context.Entry(leaveRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeaveRequestExists(id))
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

        // DELETE: api/LeaveRequestApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeaveRequest(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null)
            {
                return NotFound();
            }

            _context.LeaveRequests.Remove(leaveRequest);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LeaveRequestExists(int id)
        {
            return _context.LeaveRequests.Any(e => e.Id == id);
        }
    }
}
