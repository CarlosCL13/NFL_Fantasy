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

    /// <summary>
    /// Servicio para gestionar temporadas de la NFL.
    /// </summary>
    public class SeasonService
    {

        /// <summary>
        /// Contexto de la base de datos.
        /// </summary>
        private readonly FantasyContext _context;

        /// <summary>
        /// Constructor del servicio SeasonService.
        /// </summary>
        public SeasonService(FantasyContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Crea una nueva temporada con sus semanas.
        /// </summary>
        public async Task<(bool Success, string? Error, Season? Season)> CreateSeasonAsync(CreateSeasonDto dto)
        {
            // Valida que la fecha de fin sea posterior a la de inicio
            if (dto.EndDate <= dto.StartDate)
                return (false, "La fecha de fin debe ser posterior a la de inicio.", null);

            // Valida que las fechas no estén en el pasado
            if (dto.StartDate < DateTime.Today || dto.EndDate < DateTime.Today)
                return (false, "Las fechas no pueden estar en el pasado.", null);

            // Validar nombre único
            if (await _context.Seasons.AnyAsync(s => s.Name == dto.Name))
                return (false, "Ya existe una temporada con ese nombre.", null);

            // Validar traslapes con otras temporadas
            if (await _context.Seasons.AnyAsync(s =>
                (dto.StartDate <= s.EndDate && dto.EndDate >= s.StartDate)))
                return (false, "Las fechas se traslapan con otra temporada existente.", null);

            // Validar única temporada actual
            if (dto.IsCurrent && await _context.Seasons.AnyAsync(s => s.IsCurrent))
                return (false, "Ya existe una temporada con estado actual.", null);

            // Generar semanas
            var totalDays = (dto.EndDate - dto.StartDate).TotalDays + 1; // incluir el día final
            var daysPerWeek = Math.Floor(totalDays / dto.WeeksCount); // distribución base
            var extraDays = (int)(totalDays % dto.WeeksCount); // días adicionales a distribuir
            var weeks = new List<Week>(); // lista de semanas generadas
            var weekStart = dto.StartDate; // fecha de inicio de la primera semana

            // Crear cada semana
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
