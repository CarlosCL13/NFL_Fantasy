using Microsoft.EntityFrameworkCore;
using NFLFantasy.Api.Data;
using NFLFantasy.Api.Models;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Repositories;

namespace NFLFantasy.Api.Validators
{

    public static class NflTeamValidator
    {   

        /// <summary>
        /// Validaciones centralizadas para la creación de equipo de la NFL.
        /// </summary>
        public static async Task<(bool IsValid, string ErrorMessage)> ValidateCreateNflTeamAsync(
            string name, string city, string imageFileName, string thumbnailFileName, NflTeamRepository repository)
        {
            // Validar campos obligatorios
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(city) ||
                string.IsNullOrWhiteSpace(imageFileName) || string.IsNullOrWhiteSpace(thumbnailFileName))
            {
                return (false, AppConstants.ErrorMissingNflTeamFields);
            }

            // Validar nombre único
            if (await repository.NflTeamNameExistsAsync(name))
            {
                return (false, AppConstants.ErrorNflTeamNameExists);
            }

            return (true, string.Empty);
        }
    }
}