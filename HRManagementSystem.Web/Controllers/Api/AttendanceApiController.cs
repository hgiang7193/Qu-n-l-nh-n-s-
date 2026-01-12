using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRManagementSystem.Web.Models;
using HRManagementSystem.Web.Data;
using HRManagementSystem.Web.Models.Dtos;
using AutoMapper;

namespace HRManagementSystem.Web.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AttendanceApiController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Attendance
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AttendanceDto>>> GetAttendances()
        {
            var attendances = await _context.Attendances
                .Include(a => a.Employee)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<AttendanceDto>>(attendances));
        }

        // GET: api/Attendance/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AttendanceDto>> GetAttendance(int id)
        {
            var attendance = await _context.Attendances
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendance == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<AttendanceDto>(attendance));
        }

        // POST: api/Attendance
        [HttpPost]
        public async Task<ActionResult<AttendanceDto>> PostAttendance(CreateAttendanceDto createAttendanceDto)
        {
            var attendance = _mapper.Map<Attendance>(createAttendanceDto);
            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            var attendanceDto = _mapper.Map<AttendanceDto>(attendance);
            return CreatedAtAction(nameof(GetAttendance), new { id = attendance.Id }, attendanceDto);
        }

        // PUT: api/Attendance/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAttendance(int id, UpdateAttendanceDto updateAttendanceDto)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }

            _mapper.Map(updateAttendanceDto, attendance);
            _context.Entry(attendance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttendanceExists(id))
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

        // DELETE: api/Attendance/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }

            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AttendanceExists(int id)
        {
            return _context.Attendances.Any(e => e.Id == id);
        }
    }
}