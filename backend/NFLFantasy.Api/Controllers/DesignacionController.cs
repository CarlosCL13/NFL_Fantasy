using Microsoft.AspNetCore.Mvc;
using NFLFantasy.Api.Models;
using NFLFantasy.Api.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace NFLFantasy.Api.Controllers
{
    [ApiController]
    [Route("api/designaciones")]
    public class DesignacionController : ControllerBase
    {
        private readonly FantasyContext _context;
        public DesignacionController(FantasyContext context)
        {
            _context = context;
        }

        // GET: api/designaciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Designacion>>> GetAll()
        {
            var designaciones = await _context.Designaciones.ToListAsync();
            return Ok(designaciones);
        }

        // POST: api/designaciones
        [HttpPost]
        public async Task<ActionResult<Designacion>> Create([FromBody] Designacion dto)
        {
            _context.Designaciones.Add(dto);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = dto.Id }, dto);
        }
    }
}
