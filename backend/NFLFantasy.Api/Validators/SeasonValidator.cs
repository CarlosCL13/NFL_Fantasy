using Microsoft.EntityFrameworkCore;
using NFLFantasy.Api.Data;
using NFLFantasy.Api.Models;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.DataAccessLayer.Repositories;

namespace NFLFantasy.Api.Validators
{
    public static class SeasonValidator
    {
    
        /// <summary>
        /// Validaciones centralizadas para la creación de temporada.
        /// </summary>
        public static async Task<(bool IsValid, string? Error)> ValidateCreateSeasonAsync(
            CreateSeasonDto dto,
            FantasyContext context,
            ISeasonRepository repository)
        {
            // Valida que la fecha de fin sea posterior a la de inicio
            if (dto.EndDate <= dto.StartDate)
            {
                return (false, AppConstants.ErrorInvalidSeasonDates);
            }

            // Valida que las fechas no estén en el pasado
            if (dto.StartDate < DateTime.Today || dto.EndDate < DateTime.Today)
            {
                return (false, AppConstants.ErrorPastDates);
            }

            // Validar nombre único
            if (await repository.SeasonNameExistsAsync(dto.Name))
            {
                return (false, AppConstants.ErrorSeasonNameExists);
            }

            // Validar traslapes con otras temporadas
            if (await repository.HasDateOverlapAsync(dto.StartDate, dto.EndDate))
            {
                return (false, AppConstants.ErrorSeasonDateOverlap);
            }

            return (true, null);
        }


    }
}