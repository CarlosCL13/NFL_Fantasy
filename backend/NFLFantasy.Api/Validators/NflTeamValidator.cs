using Microsoft.EntityFrameworkCore;
using NFLFantasy.Api.Data;
using NFLFantasy.Api.Models;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.DataAccessLayer.Repositories;

namespace NFLFantasy.Api.Validators
{

    public static class NflTeamValidator
    {

        /// <summary>
        /// Validaciones centralizadas para la creación de equipo de la NFL.
        /// </summary>
        public static async Task<(bool IsValid, string ErrorMessage)> ValidateCreateNflTeamAsync(
            CreateNflTeamDto dto, NflTeamRepository repository)
        {
            // Validar campos obligatorios
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.City) || dto.Image == null)
                return (false, AppConstants.ErrorMissingNflTeamFields);

            // Validar nombre único
            if (await repository.NflTeamNameExistsAsync(dto.Name))
                return (false, AppConstants.ErrorNflTeamNameExists);

            // Validar imagen (extensión, tamaño)
            var (isImageValid, imageError) = ValidateImage(dto.Image);
            if (!isImageValid)
                return (false, imageError ?? string.Empty);

            return (true, string.Empty);
        }

        /// <summary>
        /// Valida la imagen del equipo NFL.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static (bool IsValid, string? ErrorMessage) ValidateImage(IFormFile? image)
        {
            if (image == null || image.Length == 0)
                return (false, AppConstants.ErrorRequiredImage);

            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!AppConstants.AllowedImageExtensions.Contains(extension))
                return (false, AppConstants.ErrorInvalidImageFormat);

            if (image.Length > AppConstants.MaxImageFileSize)
                return (false, AppConstants.ErrorImageTooLarge);

            return (true, null);
        }
    }
}