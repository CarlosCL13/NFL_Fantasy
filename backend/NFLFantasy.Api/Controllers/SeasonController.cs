using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Services;

namespace NFLFantasy.Api.Controllers
{
    [ApiController]
    [Route("api/seasons")]

    /// <summary>
    /// Controlador para la gestión de temporadas.
    /// </summary>
    public class SeasonController : ControllerBase
    {
        /// <summary>
        /// Servicio de temporada.
        /// </summary>
        private readonly SeasonService _seasonService;

        /// <summary>
        /// Constructor del controlador de temporada.
        /// </summary>
        public SeasonController(SeasonService seasonService)
        {
            _seasonService = seasonService;
        }

        /// <summary>
        /// Crea una nueva temporada y sus semanas (solo administrador).
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSeasonDto dto)
        {
            // Validación del modelo
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Llama al servicio para crear la temporada
            var (success, error, season) = await _seasonService.CreateSeasonAsync(dto);

            // Verificar si hubo un error al crear la temporada
            if (!success)
                return BadRequest(new { error });

            // Devuelve respuesta exitosa
            return Ok(new { message = "Temporada creada exitosamente", seasonId = season!.SeasonId });
        }

        /// <summary>
        /// Obtiene todas las temporadas existentes.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var seasons = await _seasonService.GetAllSeasonsAsync();
            return Ok(seasons);
        }

        /// <summary>
        /// Verifica si un nombre de temporada está disponible.
        /// </summary>
        [HttpGet("check-name/{name}")]
        public async Task<IActionResult> CheckNameAvailability(string name)
        {
            var isAvailable = await _seasonService.IsSeasonNameAvailableAsync(name);
            return Ok(new { 
                name = name, 
                isAvailable = isAvailable,
                message = isAvailable ? "Nombre disponible" : "Ya existe una temporada con este nombre"
            });
        }

        /// <summary>
        /// Obtiene la temporada actual activa.
        /// </summary>
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentSeason()
        {
            var currentSeason = await _seasonService.GetCurrentSeasonAsync();
            if (currentSeason == null)
                return NotFound(new { message = "No hay temporada activa" });

            return Ok(currentSeason);
        }

        /// <summary>
        /// Verifica conflictos potenciales antes de crear una temporada.
        /// </summary>
        [HttpPost("check-conflicts")]
        public async Task<IActionResult> CheckConflicts([FromBody] CreateSeasonDto dto)
        {
            var conflictInfo = await _seasonService.GetConflictInfoAsync(dto);
            return Ok(conflictInfo);
        }
    }
}
