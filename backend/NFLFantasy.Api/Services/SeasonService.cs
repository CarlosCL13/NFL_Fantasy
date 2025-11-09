using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NFLFantasy.Api.Data;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Models;
using NFLFantasy.Api.Repositories;

namespace NFLFantasy.Api.Services
{

    /// <summary>
    /// Servicio para gestionar temporadas de la NFL.
    /// </summary>
    public class SeasonService
    {

        /// <summary>
        /// Contexto de la base de datos.
        /// </summary>
        private readonly FantasyContext _context;

        private readonly NFLFantasy.Api.Repositories.SeasonRepository _repository;

        /// <summary>
        /// Constructor del servicio SeasonService.
        /// </summary>
        public SeasonService(FantasyContext context)
        {
            _context = context;
            _repository = new NFLFantasy.Api.Repositories.SeasonRepository(context);
        }

        /// <summary>
        /// Crea una nueva temporada con sus semanas.
        /// </summary>
        public async Task<(bool Success, string? Error, Season? Season)> CreateSeasonAsync(CreateSeasonDto dto)
        {
            // Validar datos de la temporada
            var (isValid, errorMessage) = await NFLFantasy.Api.Validators.SeasonValidator.ValidateCreateSeasonAsync(dto, _context, _repository);
            if (!isValid){
                return (false, errorMessage, null);
            }
            
            // Si se marca como actual, desactivar la temporada actual existente
            if (dto.IsCurrent)
            {
                await DeactivateCurrentSeasonAsync();
            }

            // Generar semanas
            var weeks = GenerateWeeks(dto);

            // Validar que no haya traslapes entre semanas
            var overlapError = ValidateWeeksNoOverlap(weeks);
            if (overlapError != null)
                return (false, overlapError, null);

            // Crear y guardar la temporada
            var season = new Season
            {
                Name = dto.Name,
                WeeksCount = dto.WeeksCount,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsCurrent = dto.IsCurrent,
                CreatedAt = DateTime.Now,
                Weeks = weeks
            };

            // Guardar en la base de datos
            await _repository.AddSeasonAsync(season);

            // Devolver resultado exitoso
            return (true, null, season);
        }

        /// <summary>
        /// Obtiene todas las temporadas existentes con sus semanas.
        /// </summary>
        public async Task<List<Season>> GetAllSeasonsAsync()
        {
            return await _context.Seasons
                .Include(s => s.Weeks)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Verifica si un nombre de temporada está disponible.
        /// </summary>
        public async Task<bool> IsSeasonNameAvailableAsync(string name)
        {   
            // Verificar disponibilidad del nombre
            return !await _context.Seasons.AnyAsync(s => s.Name.ToLower() == name.ToLower());
        }

        /// <summary>
        /// Obtiene la temporada actualmente marcada como actual.
        /// </summary>
        public async Task<Season?> GetCurrentSeasonAsync()
        {
            return await _context.Seasons
                .Include(s => s.Weeks)
                .FirstOrDefaultAsync(s => s.IsCurrent);
        }

        /// <summary>
        /// Obtiene información resumida sobre conflictos potenciales.
        /// </summary>
        public async Task<object> GetConflictInfoAsync(CreateSeasonDto dto)
        {
            // Verificar conflictos potenciales
            var nameExists = await _context.Seasons.AnyAsync(s => s.Name.ToLower() == dto.Name.ToLower());
            var currentSeasonExists = await _context.Seasons.AnyAsync(s => s.IsCurrent);
            var dateOverlap = await _context.Seasons.AnyAsync(s =>
                (dto.StartDate <= s.EndDate && dto.EndDate >= s.StartDate));

            // Devolver resumen de conflictos
            return new
            {
                nameConflict = nameExists,
                currentSeasonConflict = dto.IsCurrent && currentSeasonExists,
                dateConflict = dateOverlap,
                canCreate = !nameExists && !(dto.IsCurrent && currentSeasonExists) && !dateOverlap
            };
        }

        /// <summary>
        /// Desactiva la temporada actualmente marcada como actual.
        /// </summary>
        private async Task DeactivateCurrentSeasonAsync()
        {
            var temporadaActual = await _context.Seasons.FirstOrDefaultAsync(s => s.IsCurrent);
            if (temporadaActual != null)
            {
                temporadaActual.IsCurrent = false;
                _context.Seasons.Update(temporadaActual);
            }
        }

        /// <summary>
        /// Método para generar las semanas de la temporada.
        /// </summary>
        private List<Week> GenerateWeeks(CreateSeasonDto dto)
        {
            var totalDays = (dto.EndDate - dto.StartDate).TotalDays + 1;
            var daysPerWeek = Math.Floor(totalDays / dto.WeeksCount);
            var extraDays = (int)(totalDays % dto.WeeksCount);
            var weeks = new List<Week>();
            var weekStart = dto.StartDate;

            for (int i = 1; i <= dto.WeeksCount; i++)
            {
                var weekLength = (int)daysPerWeek + (i <= extraDays ? 1 : 0);
                var weekEnd = weekStart.AddDays(weekLength - 1);
                if (weekEnd > dto.EndDate) weekEnd = dto.EndDate;
                weeks.Add(new Week
                {
                    Number = i,
                    StartDate = weekStart,
                    EndDate = weekEnd
                });
                weekStart = weekEnd.AddDays(1);
            }
            return weeks;
        }

        /// <summary>
        /// Método para validar que no haya traslapes entre semanas.
        /// </summary>
        private string? ValidateWeeksNoOverlap(List<Week> weeks)
        {
            for (int i = 1; i < weeks.Count; i++)
            {
                if (weeks[i].StartDate <= weeks[i - 1].EndDate)
                    return $"Las semanas {weeks[i - 1].Number} y {weeks[i].Number} se traslapan.";
            }
            return null;
        }
    }
}
