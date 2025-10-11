using Microsoft.AspNetCore.Mvc;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Services;

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
        public async Task<IActionResult> Create([FromForm] CreateNflTeamDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Guardar imagen en disco
            var file = dto.Image;
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "La imagen es requerida." });

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "nflteams");
            Directory.CreateDirectory(uploadsFolder);
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Actualizar DTO para pasar la ruta de la imagen al service
            var (success, error, team) = await _nflTeamService.CreateNflTeamAsync(
                dto.Name,
                dto.City,
                uniqueFileName
            );
            if (!success)
                return BadRequest(new { error });

            return Ok(new { message = "Equipo NFL creado exitosamente", teamId = team!.NflTeamId });
        }
    }
}
