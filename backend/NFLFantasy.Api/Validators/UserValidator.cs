using Microsoft.EntityFrameworkCore;
using NFLFantasy.Api.Data;
using NFLFantasy.Api.Models;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.DataAccessLayer.Repositories;

namespace NFLFantasy.Api.Validators
{
    public static class UserValidator
    {
        /// <summary>
        /// Valida los datos para crear un nuevo usuario.
        /// </summary>
        public static async Task<(bool IsValid, string? ErrorMessage)> ValidateCreateUserAsync(RegisterUserDto dto, IUserRepository repository)
        {
            // Verificar que el correo no exista
            if (await repository.EmailExistsAsync(dto.Email))
            {
                return (false, AppConstants.ErrorEmailAlreadyRegistered);
            }

            // Verificar que el alias no exista
            if (await repository.AliasExistsAsync(dto.Alias))
            {
                return (false, AppConstants.ErrorAliasInUse);
            }

            // Validación de campos obligatorios
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Alias) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return (false, AppConstants.ErrorMissingUserFields);
            } 

            // Validar formato de la imagen de perfil
            var (isImageValid, imageError) = ValidateProfileImage(dto.ProfileImage);
            if (!isImageValid)
            {
                return (false, imageError);
            }

            return (true, null);
        }

        /// <summary>
        /// Valida la imagen de perfil del usuario.
        /// </summary>
        public static (bool IsValid, string? ErrorMessage) ValidateProfileImage(IFormFile? image)
        {
            if (image == null || image.Length == 0)
            {
                return (true, null); // No hay imagen, es válido
            }

            // Validar extensión de la imagen
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!AppConstants.AllowedImageExtensions.Contains(extension))
            {
                return (false, AppConstants.ErrorProfileImageFormat);
            }

            // Validar tamaño de la imagen
            if (image.Length > AppConstants.MaxImageFileSize)
            {
                return (false, AppConstants.ErrorProfileImageTooLarge);
            }

            return (true, null);
        }

    }
}