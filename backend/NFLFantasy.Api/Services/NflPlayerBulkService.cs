using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;
using NFLFantasy.Api.Repositories;
using NFLFantasy.Api.Validators;


namespace NFLFantasy.Api.Services
{
    public class NflPlayerBulkService
    {
        private readonly INflPlayerRepository _repository;
        private readonly NflPlayerValidator _validator;
        private readonly FantasyContext _context;
        public NflPlayerBulkService(INflPlayerRepository repository, NflPlayerValidator validator, FantasyContext context)
        {
            _repository = repository;
            _validator = validator;
            _context = context;
        }

        public async Task<BulkUploadResult> ProcessBulkAsync(List<NflPlayerBulkDto> players, string uploadsFolder, string originalFilePath, string processedFolder)
        {
            var errors = new List<string>();
            var validPlayers = new List<NflPlayerBulkDto>();
            int index = 1;
            foreach (var dto in players)
            {
                // Adaptar el DTO de bulk a un DTO de creación estándar para reusar el validador
                var createDto = new NflPlayerCreateDto
                {
                    Name = dto.Name,
                    PositionId = dto.PositionId,
                    NflTeamId = dto.NflTeamId
                };
                var (isValid, error) = _validator.ValidateCreate(createDto, _repository, requireImage: false);
                if (!isValid)
                {
                    errors.Add($"Jugador #{index} ('{dto.Name}'): {error}");
                }
                else if (string.IsNullOrWhiteSpace(dto.ImagePath))
                {
                    errors.Add($"Jugador #{index} ('{dto.Name}'): La ruta de la imagen es obligatoria.");
                }
                else if (!File.Exists(dto.ImagePath))
                {
                    errors.Add($"Jugador #{index} ('{dto.Name}'): No se encontró el archivo de imagen en la ruta especificada: {dto.ImagePath}");
                }
                else
                {
                    validPlayers.Add(dto);
                }
                index++;
            }
            if (errors.Count > 0)
            {
                return new BulkUploadResult
                {
                    Success = false,
                    Errors = errors,
                    CreatedCount = 0
                };
            }
            // Lógica de guardado, thumbnails y transacción
            var createdPlayers = new List<NFLFantasy.Api.Models.NflPlayer>();
            var successMessages = new List<string>();
            Directory.CreateDirectory(uploadsFolder);
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    int idx = 1;
                    foreach (var dto in validPlayers)
                    {
                        // Leer imagen desde ruta local
                        byte[] imageBytes;
                        try
                        {
                            imageBytes = await File.ReadAllBytesAsync(dto.ImagePath);
                        }
                        catch
                        {
                            throw new Exception($"Jugador #{idx} ('{dto.Name}'): No se pudo leer la imagen desde la ruta especificada: {dto.ImagePath}");
                        }
                        var uniqueFileName = $"{Guid.NewGuid()}_{dto.Name}.jpg";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        var thumbnailFileName = $"thumb_{Guid.NewGuid()}.png";
                        var thumbnailPath = Path.Combine(uploadsFolder, thumbnailFileName);

                        await File.WriteAllBytesAsync(filePath, imageBytes);

                        // Generar thumbnail
                        using (var image = SixLabors.ImageSharp.Image.Load(filePath))
                        {
                            image.Mutate(x => x.Resize(new SixLabors.ImageSharp.Size(100, 100)));
                            using (var thumbStream = new FileStream(thumbnailPath, FileMode.Create))
                            {
                                image.Save(thumbStream, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
                            }
                        }

                        var player = new NFLFantasy.Api.Models.NflPlayer
                        {
                            Name = dto.Name,
                            PositionId = dto.PositionId,
                            NflTeamId = dto.NflTeamId,
                            ImageUrl = uniqueFileName,
                            ThumbnailUrl = thumbnailFileName,
                            CreatedAt = DateTime.UtcNow,
                            IsActive = true
                        };
                        _context.NflPlayers.Add(player);
                        createdPlayers.Add(player);
                        successMessages.Add($"Jugador '{dto.Name}' creado correctamente.");
                        idx++;
                    }
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BulkUploadResult
                    {
                        Success = false,
                        Errors = new List<string> { ex.Message },
                        CreatedCount = 0
                    };
                }
            }
            // Mover archivo JSON a carpeta de procesados con formato <nombre>__<timestamp>.json
            string? moveWarning = null;
            try
            {
                Directory.CreateDirectory(processedFolder);
                var originalName = Path.GetFileNameWithoutExtension(originalFilePath);
                var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                var processedFileName = $"{originalName}__{timestamp}.json";
                var processedPath = Path.Combine(processedFolder, processedFileName);
                if (File.Exists(originalFilePath))
                {
                    File.Move(originalFilePath, processedPath);
                }
            }
            catch (Exception ex)
            {
                moveWarning = $"Advertencia: El archivo JSON no pudo moverse a la carpeta de procesados. Detalle: {ex.Message}";
            }

            return new BulkUploadResult
            {
                Success = true,
                Errors = new List<string>(),
                CreatedCount = createdPlayers.Count,
                SuccessMessages = successMessages,
                Warning = moveWarning
            };
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
    }
}
