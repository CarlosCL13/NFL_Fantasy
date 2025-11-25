using System;
using System.Threading.Tasks;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Models;
using NFLFantasy.Api.DataAccessLayer.Repositories;
using NFLFantasy.Api.DataAccessLayer.StorageManagement;
using NFLFantasy.Api.Validators;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;

namespace NFLFantasy.Api.Services
{
    public class NflPlayerService
    {
        private readonly INflPlayerRepository _repository;
        private readonly NflPlayerValidator _validator;
        private readonly IImageStorageHandler _imageService;
        
        public NflPlayerService(INflPlayerRepository repository, NflPlayerValidator validator, IImageStorageHandler imageService)
        {
            _repository = repository;
            _validator = validator;
            _imageService = imageService;
        }

        /// <summary>
        /// Crea un jugador NFL desde IFormFile (para uploads vía API).
        /// </summary>
        public async Task<(bool Success, string? Error)> CreateNflPlayerAsync(NflPlayerCreateDto dto, IFormFile imageFile, string uploadsFolder)
        {
            // Procesar imagen y generar thumbnail usando el servicio compartido
            var (uniqueFileName, thumbnailFileName) = await _imageService.ProcessImageAsync(imageFile, uploadsFolder);
            
            return await CreatePlayerInternalAsync(dto, uniqueFileName, thumbnailFileName, requireImageValidation: true);
        }

        /// <summary>
        /// Crea un jugador NFL desde byte array (para bulk upload).
        /// </summary>
        public async Task<(bool Success, string? Error)> CreateNflPlayerAsync(NflPlayerCreateDto dto, byte[] imageBytes, string imageName, string uploadsFolder)
        {
            // Procesar imagen y generar thumbnail usando el servicio compartido
            var (uniqueFileName, thumbnailFileName) = await _imageService.ProcessImageAsync(imageBytes, imageName, uploadsFolder);
            
            return await CreatePlayerInternalAsync(dto, uniqueFileName, thumbnailFileName, requireImageValidation: false);
        }

        /// <summary>
        /// Método interno que contiene la lógica común de creación de jugador.
        /// </summary>
        private async Task<(bool Success, string? Error)> CreatePlayerInternalAsync(
            NflPlayerCreateDto dto, 
            string imageFileName, 
            string thumbnailFileName, 
            bool requireImageValidation)
        {
            var (isValid, error) = _validator.ValidateCreate(dto, _repository, requireImageValidation);
            if (!isValid)
                return (false, error);

            var player = new NflPlayer
            {
                Name = dto.Name,
                PositionId = dto.PositionId,
                NflTeamId = dto.NflTeamId,
                ImageUrl = imageFileName,
                ThumbnailUrl = thumbnailFileName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            
            await _repository.AddAsync(player);
            return (true, null);
        }
    }
}
