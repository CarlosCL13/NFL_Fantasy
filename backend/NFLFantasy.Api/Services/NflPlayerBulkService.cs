using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Data;
using NFLFantasy.Api.DataAccessLayer.Repositories;
using NFLFantasy.Api.DataAccessLayer.StorageManagement;
using NFLFantasy.Api.DataAccessLayer.FileManagement;
using NFLFantasy.Api.Validators;
using System.Collections.Generic;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;


namespace NFLFantasy.Api.Services
{
    public class NflPlayerBulkService
    {
        private readonly NflPlayerService _playerService;
        private readonly JsonFileHandler _jsonFileService;
        private readonly FantasyContext _context;
        
        public NflPlayerBulkService(
            NflPlayerService playerService,
            JsonFileHandler jsonFileService,
            FantasyContext context)
        {
            _playerService = playerService;
            _jsonFileService = jsonFileService;
            _context = context;
        }

        /// <summary>
        /// Procesa la carga masiva de jugadores NFL desde una lista de DTOs.
        /// </summary>
        public async Task<BulkUploadResult> ProcessBulkAsync(List<NflPlayerBulkDto> players, string uploadsFolder, string originalFilePath, string processedFolder)
        {
            var errors = new List<string>();
            var successMessages = new List<string>();
            var createdCount = 0;
            
            // Validar que todas las imágenes existan antes de procesar
            var imageValidationErrors = ValidateImageFilesExist(players);
            if (imageValidationErrors.Any())
            {
                return await CreateErrorResult(imageValidationErrors, originalFilePath, processedFolder);
            }

            // Procesar cada jugador usando el servicio individual dentro de una transacción
            Directory.CreateDirectory(uploadsFolder);
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    int idx = 1;
                    foreach (var dto in players)
                    {
                        var (playerCreated, playerErrors, playerSuccess) = await ProcessSinglePlayerAsync(dto, uploadsFolder, idx);
                        
                        if (playerCreated)
                        {
                            createdCount++;
                            successMessages.AddRange(playerSuccess);
                        }
                        
                        errors.AddRange(playerErrors);
                        idx++;
                    }
                    
                    // Si hubo errores de validación, hacer rollback (todo o nada)
                    if (errors.Count > 0)
                    {
                        transaction.Rollback();
                        return await CreateErrorResult(errors, originalFilePath, processedFolder);
                    }
                    
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return await CreateErrorResult(new List<string> { ex.Message }, originalFilePath, processedFolder);
                }
            }

            // Mover archivo exitoso
            return await CreateSuccessResult(createdCount, successMessages, originalFilePath, processedFolder);
        }

        /// <summary>
        /// Valida que todas las imágenes existan en el disco antes de procesarlas.
        /// </summary>
        private List<string> ValidateImageFilesExist(List<NflPlayerBulkDto> players)
        {
            var errors = new List<string>();
            int index = 1;
            
            foreach (var dto in players)
            {
                if (string.IsNullOrWhiteSpace(dto.ImagePath))
                {
                    errors.Add($"Jugador #{index} ('{dto.Name}'): La ruta de la imagen es obligatoria.");
                }
                else if (!File.Exists(dto.ImagePath))
                {
                    errors.Add($"Jugador #{index} ('{dto.Name}'): No se encontró el archivo de imagen en la ruta especificada: {dto.ImagePath}");
                }
                index++;
            }
            
            return errors;
        }

        /// <summary>
        /// Procesa un solo jugador dentro de la transacción.
        /// Retorna: (playerCreated, errors, successMessages)
        /// </summary>
        private async Task<(bool PlayerCreated, List<string> Errors, List<string> SuccessMessages)> ProcessSinglePlayerAsync(
            NflPlayerBulkDto dto, 
            string uploadsFolder, 
            int index)
        {
            var errors = new List<string>();
            var successMessages = new List<string>();
            
            try
            {
                // Leer imagen desde disco
                byte[] imageBytes = await File.ReadAllBytesAsync(dto.ImagePath);
                
                // Crear DTO para validación
                var playerDto = new NflPlayerCreateDto
                {
                    Name = dto.Name,
                    PositionId = dto.PositionId,
                    NflTeamId = dto.NflTeamId,
                    Image = null! // No aplica para bulk
                };
                
                // Usar el servicio individual para crear el jugador
                var (success, error) = await _playerService.CreateNflPlayerAsync(playerDto, imageBytes, dto.Name, uploadsFolder);
                
                if (!success)
                {
                    errors.Add($"Jugador #{index} ('{dto.Name}'): {error}");
                    return (false, errors, successMessages);
                }
                
                successMessages.Add($"Jugador '{dto.Name}' creado correctamente.");
                return (true, errors, successMessages);
            }
            catch (Exception ex)
            {
                errors.Add($"Jugador #{index} ('{dto.Name}'): Error al procesar - {ex.Message}");
                return (false, errors, successMessages);
            }
        }

        /// <summary>
        /// Crea un resultado de error y mueve el archivo con sufijo _ERROR.
        /// </summary>
        private Task<BulkUploadResult> CreateErrorResult(List<string> errors, string originalFilePath, string processedFolder)
        {
            var (success, processedPath, errorMsg) = _jsonFileService.MoveToProcessedFolder(originalFilePath, processedFolder, hasErrors: true);
            
            return Task.FromResult(new BulkUploadResult
            {
                Success = false,
                Errors = errors,
                CreatedCount = 0,
                ProcessedFilePath = processedPath,
                Warning = errorMsg
            });
        }

        /// <summary>
        /// Crea un resultado exitoso y mueve el archivo sin sufijo de error.
        /// </summary>
        private Task<BulkUploadResult> CreateSuccessResult(int createdCount, List<string> successMessages, string originalFilePath, string processedFolder)
        {
            var (moveSuccess, movedPath, moveWarning) = _jsonFileService.MoveToProcessedFolder(originalFilePath, processedFolder, hasErrors: false);

            return Task.FromResult(new BulkUploadResult
            {
                Success = true,
                Errors = new List<string>(),
                CreatedCount = createdCount,
                SuccessMessages = successMessages,
                Warning = moveWarning,
                ProcessedFilePath = movedPath
            });
        }
    }

    /// <summary>
    /// Resultado del proceso de carga masiva.
    /// </summary>
    public class BulkUploadResult
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new();
        public int CreatedCount { get; set; }
        public List<string> SuccessMessages { get; set; } = new();
        public string? Warning { get; set; }
        public string? ProcessedFilePath { get; set; }
    }
}
