using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace NFLFantasy.Api.Controllers
{
    [ApiController]
    [Route("api/playernews")]
    public class PlayerNewsController : ControllerBase
    {
        private readonly PlayerNewsService _service;
        public PlayerNewsController(PlayerNewsService service)
        {
            _service = service;
        }

        /// <summary>
        /// Agrega una noticia para un jugador NFL (solo administrador).
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> AddNews([FromBody] CreatePlayerNewsDto dto)
        {
            // Obtener el autor del contexto de usuario autenticado
            string autor = "admin"; // Placeholder
            var (success, errors) = await _service.AddNewsAsync(dto, autor);
            if (!success)
                return BadRequest(new { errors });
            return Ok(new { message = "Noticia agregada correctamente." });
        }

        /// <summary>
        /// Obtiene las noticias de un jugador NFL por su ID.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpGet("{playerId}")]
        public async Task<IActionResult> GetNews(int playerId)
        {
            var news = await _service.GetNewsByPlayerAsync(playerId);
            return Ok(news);
        }
    }
}
