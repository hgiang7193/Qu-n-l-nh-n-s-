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
    public class PositionApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PositionApiController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/PositionApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PositionDto>>> GetPositions()
        {
            var positions = await _context.Positions.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<PositionDto>>(positions));
        }

        // GET: api/PositionApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PositionDto>> GetPosition(int id)
        {
            var position = await _context.Positions.FindAsync(id);

            if (position == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PositionDto>(position));
        }

        // POST: api/PositionApi
        [HttpPost]
        public async Task<ActionResult<PositionDto>> PostPosition(CreatePositionDto createPositionDto)
        {
            var position = _mapper.Map<Position>(createPositionDto);
            _context.Positions.Add(position);
            await _context.SaveChangesAsync();

            var positionDto = _mapper.Map<PositionDto>(position);

            return CreatedAtAction(nameof(GetPosition), new { id = position.Id }, positionDto);
        }

        // PUT: api/PositionApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPosition(int id, UpdatePositionDto updatePositionDto)
        {
            var position = await _context.Positions.FindAsync(id);
            if (position == null)
            {
                return NotFound();
            }

            _mapper.Map(updatePositionDto, position);
            _context.Entry(position).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PositionExists(id))
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

        // DELETE: api/PositionApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePosition(int id)
        {
            var position = await _context.Positions.FindAsync(id);
            if (position == null)
            {
                return NotFound();
            }

            _context.Positions.Remove(position);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PositionExists(int id)
        {
            return _context.Positions.Any(e => e.Id == id);
        }
    }
}