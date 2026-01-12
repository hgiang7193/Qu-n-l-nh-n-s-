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
    public class ShiftApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ShiftApiController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Shift
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShiftDto>>> GetShifts()
        {
            return await _context.Shifts
                .ProjectTo<ShiftDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // GET: api/Shift/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShiftDto>> GetShift(int id)
        {
            var shift = await _context.Shifts
                .ProjectTo<ShiftDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shift == null)
            {
                return NotFound();
            }

            return shift;
        }

        // POST: api/Shift
        [HttpPost]
        public async Task<ActionResult<ShiftDto>> PostShift(CreateShiftDto createShiftDto)
        {
            var shift = _mapper.Map<Shift>(createShiftDto);

            _context.Shifts.Add(shift);
            await _context.SaveChangesAsync();

            var shiftDto = _mapper.Map<ShiftDto>(shift);

            return CreatedAtAction(nameof(GetShift), new { id = shift.Id }, shiftDto);
        }

        // PUT: api/Shift/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShift(int id, UpdateShiftDto updateShiftDto)
        {
            var shift = await _context.Shifts.FindAsync(id);

            if (shift == null)
            {
                return NotFound();
            }

            _mapper.Map(updateShiftDto, shift);
            _context.Entry(shift).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShiftExists(id))
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

        // DELETE: api/Shift/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShift(int id)
        {
            var shift = await _context.Shifts.FindAsync(id);
            if (shift == null)
            {
                return NotFound();
            }

            _context.Shifts.Remove(shift);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ShiftExists(int id)
        {
            return _context.Shifts.Any(e => e.Id == id);
        }
    }
}