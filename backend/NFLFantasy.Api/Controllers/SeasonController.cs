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
    }
}
