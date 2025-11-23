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
            NFLFantasy.Api.DTO.CreateSeasonDto dto,
            NFLFantasy.Api.Data.FantasyContext context,
            NFLFantasy.Api.DataAccessLayer.Repositories.SeasonRepository repository)
        {
            // Valida que la fecha de fin sea posterior a la de inicio
            if (dto.EndDate <= dto.StartDate)
            {
                return (false, "La fecha de fin debe ser posterior a la de inicio.");
            }

            // Valida que las fechas no estén en el pasado
            if (dto.StartDate < DateTime.Today || dto.EndDate < DateTime.Today)
            {
                return (false, "Las fechas no pueden estar en el pasado.");
            }

            // Validar nombre único
            if (await repository.SeasonNameExistsAsync(dto.Name))
            {
                return (false, "Ya existe una temporada con ese nombre.");
            }

            // Validar traslapes con otras temporadas
            if (await repository.HasDateOverlapAsync(dto.StartDate, dto.EndDate))
            {
                return (false, "Las fechas se traslapan con otra temporada existente.");
            }

            return (true, null);
        }


    }
}