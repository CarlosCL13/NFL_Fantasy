using Microsoft.AspNetCore.Mvc;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Services;

namespace NFLFantasy.Api.Controllers
{
    [ApiController]
    [Route("api/seasons")]
    public class SeasonController : ControllerBase
    {
        private readonly SeasonService _seasonService;
        public SeasonController(SeasonService seasonService)
        {
            _seasonService = seasonService;
        }

        /// <summary>
        /// Crea una nueva temporada y sus semanas (solo administrador).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSeasonDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, error, season) = await _seasonService.CreateSeasonAsync(dto);
            if (!success)
                return BadRequest(new { error });

            return Ok(new { message = "Temporada creada exitosamente", seasonId = season!.SeasonId });
        }
    }
}
