using Microsoft.EntityFrameworkCore;
using NFLFantasy.Api.Data;
using NFLFantasy.Api.Models;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Repositories;

namespace NFLFantasy.Api.Validators
{
    public static class UserValidator
    {
        /// <summary>
        /// Valida los datos para crear un nuevo usuario.
        /// </summary>
        public static async Task<(bool IsValid, string? ErrorMessage)> ValidateCreateUserAsync(RegisterUserDto dto, UserRepository repository)
        {
            // Verificar que el correo no exista
            if (await repository.EmailExistsAsync(dto.Email))
            {
                return (false, "Ya existe un usuario con este correo electrónico.");
            }

            // Verificar que el alias no exista
            if (await repository.AliasExistsAsync(dto.Alias))
            {
                return (false, "Ya existe un usuario con este alias.");
            }

            // Validación de campos obligatorios
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Alias) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return (false, AppConstants.ErrorMissingUserFields);
            }

            return (true, null);
        }
    }
}