using Microsoft.EntityFrameworkCore;
using NFLFantasy.Api.Data;
using NFLFantasy.Api.Models;

namespace NFLFantasy.Api.DataAccessLayer.Repositories {
    public interface INflTeamRepository
    {
        Task<bool> NflTeamNameExistsAsync(string name);
        Task AddNflTeamAsync(NflTeam team);
    }

    public class NflTeamRepository : INflTeamRepository
    {
        private readonly FantasyContext _context;
        public NflTeamRepository(FantasyContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Verifica si ya existe un equipo NFL con el nombre dado.
        /// </summary>
        public async Task<bool> NflTeamNameExistsAsync(string name)
        {
            return await _context.NflTeams.AnyAsync(t => t.Name == name);
        }

        /// <summary>
        /// Agrega un nuevo equipo NFL a la base de datos.
        /// </summary>
        public async Task AddNflTeamAsync(NflTeam team)
        {
            _context.NflTeams.Add(team);
            await _context.SaveChangesAsync();
        }
    }
}