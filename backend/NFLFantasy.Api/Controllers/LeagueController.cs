using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Services;
using System.Security.Claims;

namespace NFLFantasy.Api.Controllers
{
    [ApiController]
    [Route("api/leagues")]
    [Authorize]
    public class LeagueController : ControllerBase
    {
        private readonly LeagueService _leagueService;
        public LeagueController(LeagueService leagueService)
        {
            _leagueService = leagueService;
        }

        /// <summary>
        /// Busca ligas por nombre, temporada y estado.
        /// </summary>
        /// <param name="dto">DTO con filtros de búsqueda: nombre, temporada y estado.</param>
        /// <returns>Lista de ligas que cumplen los filtros.</returns>
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search([FromQuery] SearchLeagueDto dto)
        {
            var leagues = await _leagueService.SearchLeaguesAsync(dto);
            return Ok(leagues);
        }

        /// <summary>
        /// Permite a un usuario unirse a una liga con contraseña y validaciones.
        /// </summary>
        /// <param name="dto">DTO con datos de unión: id de liga, contraseña, alias y nombre de equipo.</param>
        /// <returns>Mensaje de éxito o error según la validación.</returns>
        [HttpPost("join")]
        public async Task<IActionResult> Join([FromBody] JoinLeagueDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { error = "No se encontró el identificador de usuario en el token JWT. Por favor, vuelve a iniciar sesión." });
            int userId;
            if (!int.TryParse(userIdClaim, out userId))
                return Unauthorized(new { error = "El identificador de usuario en el token JWT no es válido." });

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { error = "Datos de entrada inválidos.", details = errors });
            }

            var (success, error) = await _leagueService.JoinLeagueAsync(userId, dto);
            if (!success)
                return BadRequest(new { error = error ?? "No se pudo unir a la liga." });

            return Ok(new { message = "Te has unido exitosamente a la liga." });
        }

        /// <summary>
        /// Crea una nueva liga y asigna al usuario como comisionado principal.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLeagueDto dto)
        {
            // Obtiene el userId real del usuario autenticado desde el token JWT, con validación
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { error = "No se encontró el identificador de usuario en el token JWT. Por favor, vuelve a iniciar sesión." });
            int userId;
            if (!int.TryParse(userIdClaim, out userId))
                return Unauthorized(new { error = "El identificador de usuario en el token JWT no es válido." });

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { error = "Datos de entrada inválidos.", details = errors });
            }

            var (success, error, league, remainingSpots) = await _leagueService.CreateLeagueAsync(dto, userId);
            if (!success)
            {
                return BadRequest(new { error = error ?? "No se pudo crear la liga por un error desconocido." });
            }

            return Ok(new
            {
                message = "Liga creada exitosamente.",
                leagueId = league!.LeagueId,
                remainingSpots
            });
        }
    }
}
