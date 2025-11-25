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
        private readonly IDirectoryManager _directoryManager;

        public NflPlayerController(NflPlayerService nflPlayerService, NflPlayerBulkService nflPlayerBulkService, IDirectoryManager directoryManager)
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

            var result = await _nflPlayerBulkService.HandleBulkUploadAsync(file);
            if (!result.Success)
                return BadRequest(new { errors = result.Errors, warning = result.Warning });

            return Ok(new {
                message = $"{result.CreatedCount} jugadores creados exitosamente.",
                detalles = result.SuccessMessages,
                warning = result.Warning
            });
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
