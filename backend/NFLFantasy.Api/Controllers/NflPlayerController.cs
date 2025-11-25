using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Services;
using NFLFantasy.Api.DataAccessLayer.StorageManagement;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace NFLFantasy.Api.Controllers
{
    [ApiController]
    [Route("api/nflplayers")]
    public class NflPlayerController : ControllerBase
    {
        private readonly NflPlayerService _nflPlayerService;
        private readonly NflPlayerBulkService _nflPlayerBulkService;
        private readonly DirectoryManager _directoryManager;

        public NflPlayerController(NflPlayerService nflPlayerService, NflPlayerBulkService nflPlayerBulkService, DirectoryManager directoryManager)
        {
            _nflPlayerService = nflPlayerService;
            _nflPlayerBulkService = nflPlayerBulkService;
            _directoryManager = directoryManager;
        }

        /// <summary>
        /// Carga masiva de jugadores NFL desde un archivo JSON.
        /// </summary>
        [HttpPost("bulk-upload")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> BulkUpload([FromForm] NFLFantasy.Api.DTO.BulkUploadRequest request)
        {
            var file = request.File;
            if (file == null || file.Length == 0)
                return BadRequest("Debe adjuntar un archivo JSON.");

            List<NFLFantasy.Api.DTO.NflPlayerBulkDto>? players;
            using (var stream = new StreamReader(file.OpenReadStream()))
            {
                var json = await stream.ReadToEndAsync();
                try
                {
                    players = System.Text.Json.JsonSerializer.Deserialize<List<NFLFantasy.Api.DTO.NflPlayerBulkDto>>(json);
                }
                catch
                {
                    return BadRequest("El archivo no tiene formato JSON válido.");
                }
            }
            if (players == null || players.Count == 0)
                return BadRequest("El archivo no contiene datos de jugadores.");

            var uploadsFolder = _directoryManager.GetNflPlayersImagesPath();
            var jsonUploadsFolder = _directoryManager.GetNflPlayersUploadsPath();
            var jsonProcessedFolder = _directoryManager.GetNflPlayersProcessedPath();
            _directoryManager.EnsureDirectoryExists(jsonUploadsFolder);
            _directoryManager.EnsureDirectoryExists(jsonProcessedFolder);

            // Guardar el archivo JSON subido en wwwroot/uploads
            var uniqueFileName = _directoryManager.GenerateUniqueFileName(Path.GetFileNameWithoutExtension(file.FileName), ".json");
            var jsonUploadPath = Path.Combine(jsonUploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(jsonUploadPath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            var result = await _nflPlayerBulkService.ProcessBulkAsync(players, uploadsFolder, jsonUploadPath, jsonProcessedFolder);
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors, warning = result.Warning });
            
            return Ok(new { 
                message = $"{result.CreatedCount} jugadores creados exitosamente.", 
                detalles = result.SuccessMessages,
                warning = result.Warning
            });
        }

        /// <summary>
        /// Consulta todos los jugadores NFL.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var players = await _nflPlayerService.GetAllAsync();
            return Ok(players);
        }

        /// <summary>
        /// Crea un nuevo jugador NFL manualmente.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromForm] NflPlayerCreateDto dto)
        {
            var uploadsFolder = _directoryManager.GetNflPlayersImagesPath();
            var (success, error) = await _nflPlayerService.CreateNflPlayerAsync(dto, dto.Image, uploadsFolder);
            if (!success)
            {
                return BadRequest(new { error });
            }
            return Ok(new { message = "Jugador NFL creado exitosamente." });
        }
    }
}
