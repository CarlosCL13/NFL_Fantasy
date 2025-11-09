using Microsoft.EntityFrameworkCore;
using NFLFantasy.Api.Data;
using NFLFantasy.Api.Models;

namespace NFLFantasy.Api.Repositories
{
    public class NflTeamRepository
    {
        private readonly FantasyContext _context;
        public NflTeamRepository(FantasyContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Verifica si ya existe un equipo de la NFL con el nombre dado.
        /// </summary>
        public async Task<bool> NflTeamNameExistsAsync(string name)
        {
            return await _context.NflTeams.AnyAsync(t => t.Name == name);
        }

        /// <summary>
        /// Agrega un nuevo equipo de la NFL a la base de datos.
        /// </summary>
        public async Task AddNflTeamAsync(NflTeam team)
        {
            _context.NflTeams.Add(team);
            await _context.SaveChangesAsync();
        }
    }
}