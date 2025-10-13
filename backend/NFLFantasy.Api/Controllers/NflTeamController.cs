using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
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

            // Generar thumbnail usando ImageSharp
            var thumbnailFileName = $"thumb_{Guid.NewGuid()}.png";
            var thumbnailPath = Path.Combine(uploadsFolder, thumbnailFileName);
            using (var image = Image.Load(filePath))
            {
                image.Mutate(x => x.Resize(100, 100)); // tamaño del thumbnail
                image.Save(thumbnailPath);
            }

            // Actualizar DTO para pasar la ruta de la imagen y thumbnail al service
            var (success, error, team) = await _nflTeamService.CreateNflTeamAsync(
                dto.Name,
                dto.City,
                uniqueFileName,
                thumbnailFileName
            );
            if (!success)
                return BadRequest(new { error });

            return Ok(new { message = "Equipo NFL creado exitosamente", teamId = team!.NflTeamId });
        }

        /// <summary>
        /// Obtiene la lista de todos los equipos NFL creados.
        /// </summary>
        /// <returns>Lista de equipos NFL.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var teams = await _nflTeamService.GetAllNflTeamsAsync();
            var result = teams.Select(t => new {
                t.NflTeamId,
                t.Name,
                t.City,
                imageUrl = $"/images/nflteams/{t.Image}",
                thumbnailUrl = $"/images/nflteams/{t.Thumbnail}",
                t.CreatedAt,
                t.IsActive
            });
            return Ok(result);
        }
    }
}
