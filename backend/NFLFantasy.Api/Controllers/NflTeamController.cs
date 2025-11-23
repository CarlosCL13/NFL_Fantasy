using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Services;
using NFLFantasy.Api;

namespace NFLFantasy.Api.Controllers
{
    [ApiController]
    [Route("api/nflteams")]
    public class NflTeamController : ControllerBase
    {
        private readonly NflTeamService _nflTeamService;
        public NflTeamController(NflTeamService nflTeamService)
        {
            _nflTeamService = nflTeamService;
        }

        /// <summary>
        /// Crea un nuevo equipo NFL manualmente (solo administrador).
        /// </summary>
        /// <param name="dto">Datos del equipo NFL.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromForm] CreateNflTeamDto dto)
        {
            // Validación del modelo
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new { error = AppConstants.ErrorInvalidTeamData, detalles = errors });
            }

            // Llama al servicio, que se encarga de validación y guardado
            var (success, error, team) = await _nflTeamService.CreateNflTeamAsync(dto);

            if (!success)
                return BadRequest(new { error = error ?? "No se pudo crear el equipo NFL. Por favor, verifica los datos e inténtalo de nuevo." });

            return Ok(new { message = "Equipo NFL creado exitosamente.", teamId = team!.NflTeamId });
        }

        /// <summary>
        /// Obtiene la lista de todos los equipos NFL creados.
        /// </summary>
        /// <returns>Lista de equipos NFL.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Llama al servicio para obtener todos los equipos NFL
            var teams = await _nflTeamService.GetAllNflTeamsAsync();

            // Mapear los equipos a un formato DTO
            var result = teams.Select(t => new
            {
                t.NflTeamId,
                t.Name,
                t.City,
                imageUrl = $"/images/nflteams/{t.Image}",
                thumbnailUrl = $"/images/nflteams/{t.Thumbnail}",
                t.CreatedAt,
                t.IsActive
            });

            // Devuelve la lista de equipos NFL
            return Ok(result);
        }
    }
}
