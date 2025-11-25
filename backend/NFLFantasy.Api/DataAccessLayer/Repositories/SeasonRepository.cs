using Microsoft.EntityFrameworkCore;
using NFLFantasy.Api.Data;
using NFLFantasy.Api.Models;

namespace NFLFantasy.Api.DataAccessLayer.Repositories
{
    public interface ISeasonRepository
    {
        Task<bool> SeasonNameExistsAsync(string name);
        Task<bool> HasDateOverlapAsync(DateTime startDate, DateTime endDate);
        Task AddSeasonAsync(Season season);
    }

    public class SeasonRepository : ISeasonRepository
    {
        private readonly FantasyContext _context;
        public SeasonRepository(FantasyContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Verifica si ya existe una temporada con el nombre dado.
        /// </summary>
        public async Task<bool> SeasonNameExistsAsync(string name)
        {
            return await _context.Seasons.AnyAsync(s => s.Name == name);
        }

        /// <summary>
        /// Verifica si hay traslape de fechas con temporadas existentes.
        /// </summary>
        public async Task<bool> HasDateOverlapAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Seasons.AnyAsync(s =>
                (startDate <= s.EndDate && endDate >= s.StartDate));
        }

        /// <summary>
        /// Agrega una nueva temporada a la base de datos.
        /// </summary>
        public async Task AddSeasonAsync(Season season)
        {
            _context.Seasons.Add(season);
            await _context.SaveChangesAsync();
        }
    }
}
