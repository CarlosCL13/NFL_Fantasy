using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
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
        [Authorize]
        public async Task<IActionResult> Create([FromForm] CreateNflTeamDto dto)
        {
            // Validación del modelo
            if (!ModelState.IsValid)
            {
                // Extrae los errores de validación del modelo
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                // Devuelve un BadRequest con los errores detallados
                return BadRequest(new { error = AppConstants.ErrorInvalidTeamData, detalles = errors });
            }

            // Archivo de imagen subido
            var file = dto.Image;

            // Validar que se haya subido un archivo
            if (file == null || file.Length == 0)
                return BadRequest(new { error = AppConstants.ErrorRequiredImage });

            // Validar y guardar la imagen
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AppConstants.AllowedImageExtensions.Contains(extension))
                return BadRequest(new { error = AppConstants.ErrorInvalidImageFormat });

            // Validar tamaño del archivo
            if (file.Length > AppConstants.MaxImageFileSize)
                return BadRequest(new { error = AppConstants.ErrorImageTooLarge });

            // Asegurarse de que la carpeta de carga exista
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), AppConstants.NflTeamsImageFolder.Replace("/", Path.DirectorySeparatorChar.ToString()));

            // Crea el directorio si no existe
            Directory.CreateDirectory(uploadsFolder);

            // Generar un nombre de archivo único
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";

            // Ruta completa del archivo
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Guardar el archivo en el servidor
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Guardar como PNG
            var thumbnailFileName = $"thumb_{Guid.NewGuid()}.png";

            // Ruta completa del thumbnail
            var thumbnailPath = Path.Combine(uploadsFolder, thumbnailFileName);
            
            // Generar thumbnail usando ImageSharp
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

            // Verificar si hubo un error al crear el equipo NFL
            if (!success)
                return BadRequest(new { error = error ?? "No se pudo crear el equipo NFL. Por favor, verifica los datos e inténtalo de nuevo." });

            // Devuelve respuesta exitosa
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
