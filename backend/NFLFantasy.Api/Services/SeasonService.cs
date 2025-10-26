using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NFLFantasy.Api.Data;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Models;

namespace NFLFantasy.Api.Services
{
    public class SeasonService
    {
        private readonly FantasyContext _context;
        public SeasonService(FantasyContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string? Error, Season? Season)> CreateSeasonAsync(CreateSeasonDto dto)
        {
            // Validaciones básicas
            if (dto.EndDate <= dto.StartDate)
                return (false, "La fecha de fin debe ser posterior a la de inicio.", null);
            if (dto.StartDate < DateTime.Today || dto.EndDate < DateTime.Today)
                return (false, "Las fechas no pueden estar en el pasado.", null);
            if (await _context.Seasons.AnyAsync(s => s.Name == dto.Name))
                return (false, "Ya existe una temporada con ese nombre.", null);
            if (await _context.Seasons.AnyAsync(s =>
                (dto.StartDate <= s.EndDate && dto.EndDate >= s.StartDate)))
                return (false, "Las fechas se traslapan con otra temporada existente.", null);
            if (dto.IsCurrent && await _context.Seasons.AnyAsync(s => s.IsCurrent))
                return (false, "Ya existe una temporada con estado actual.", null);

            // Generar semanas
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
            // Validar traslapes entre semanas
            for (int i = 1; i < weeks.Count; i++)
            {
                if (weeks[i].StartDate <= weeks[i - 1].EndDate)
                    return (false, $"Las semanas {weeks[i - 1].Number} y {weeks[i].Number} se traslapan.", null);
            }

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
            _context.Seasons.Add(season);
            await _context.SaveChangesAsync();
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
            var nameExists = await _context.Seasons.AnyAsync(s => s.Name.ToLower() == dto.Name.ToLower());
            var currentSeasonExists = await _context.Seasons.AnyAsync(s => s.IsCurrent);
            var dateOverlap = await _context.Seasons.AnyAsync(s =>
                (dto.StartDate <= s.EndDate && dto.EndDate >= s.StartDate));

            return new
            {
                nameConflict = nameExists,
                currentSeasonConflict = dto.IsCurrent && currentSeasonExists,
                dateConflict = dateOverlap,
                canCreate = !nameExists && !(dto.IsCurrent && currentSeasonExists) && !dateOverlap
            };
        }
    }
}
