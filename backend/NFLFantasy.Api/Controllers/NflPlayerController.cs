using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Services;

namespace NFLFantasy.Api.Controllers
{
    [ApiController]
    [Route("api/nflplayers")]
    public class NflPlayerController : ControllerBase
    {
        private readonly NflPlayerService _nflPlayerService;
        public NflPlayerController(NflPlayerService nflPlayerService)
        {
            _nflPlayerService = nflPlayerService;
        }

        /// <summary>
        /// Crea un nuevo jugador NFL manualmente.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromForm] NflPlayerCreateDto dto)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "nflplayers");
            var (success, error) = await _nflPlayerService.CreateNflPlayerAsync(dto, dto.Image, uploadsFolder);
            if (!success)
            {
                return BadRequest(new { error });
            }
            return Ok(new { message = "Jugador NFL creado exitosamente." });
        }
    }
}
