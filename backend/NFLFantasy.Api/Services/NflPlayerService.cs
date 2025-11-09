using System;
using System.Threading.Tasks;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Models;
using NFLFantasy.Api.Repositories;
using NFLFantasy.Api.Validators;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;

namespace NFLFantasy.Api.Services
{
    public class NflPlayerService
    {
        private readonly NflPlayerRepository _repository;
        private readonly NflPlayerValidator _validator;
        public NflPlayerService(NflPlayerRepository repository, NflPlayerValidator validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<(bool Success, string? Error)> CreateNflPlayerAsync(NflPlayerCreateDto dto, IFormFile imageFile, string uploadsFolder)
        {
            var (isValid, error) = _validator.ValidateCreate(dto, _repository);
            if (!isValid)
                return (false, error);

            // La validación de archivo se realiza en el validador

            // Generar nombres únicos
            var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            var thumbnailFileName = $"thumb_{Guid.NewGuid()}.png";
            var thumbnailPath = Path.Combine(uploadsFolder, thumbnailFileName);

            // Guardar imagen original
            Directory.CreateDirectory(uploadsFolder);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Generar thumbnail usando ImageSharp
            using (var image = SixLabors.ImageSharp.Image.Load(filePath))
            {
                image.Mutate(x => x.Resize(new SixLabors.ImageSharp.Size(100, 100)));
                using (var thumbStream = new FileStream(thumbnailPath, FileMode.Create))
                {
                    image.Save(thumbStream, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
                }
            }

            var player = new NflPlayer
            {
                Name = dto.Name,
                PositionId = dto.PositionId,
                NflTeamId = dto.NflTeamId,
                ImageUrl = uniqueFileName,
                ThumbnailUrl = thumbnailFileName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            await _repository.AddAsync(player);
            return (true, null);
        }
    }
}
