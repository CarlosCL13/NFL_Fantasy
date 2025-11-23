using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NFLFantasy.Api.Data;
using NFLFantasy.Api.Models;

namespace NFLFantasy.Api.DataAccessLayer.Repositories
{
    public class LeagueRepository
    {
        private readonly FantasyContext _context;
        public LeagueRepository(FantasyContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Verifica si ya existe una liga con el nombre dado.
        /// </summary>
        public async Task<bool> LeagueNameExistsAsync(string name)
        {
            return await _context.Leagues.AnyAsync(l => l.Name == name);
        }

        /// <summary>
        /// Obtiene la temporada actual.
        /// </summary>
        public async Task<Season?> GetCurrentSeasonAsync()
        {
            return await _context.Seasons.FirstOrDefaultAsync(s => s.IsCurrent);
        }

        /// <summary>
        /// Agrega una nueva liga a la base de datos.
        /// </summary>
        public async Task AddLeagueAsync(League league)
        {
            _context.Leagues.Add(league);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Agrega un nuevo equipo a la base de datos.
        /// </summary>
        public async Task AddTeamAsync(Team team)
        {
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Verifica si ya existe un equipo con el alias dado.
        /// </summary>
        public async Task<bool> AliasExistsAsync(string alias)
        {
            return await _context.Teams.AnyAsync(t => t.Alias == alias);
        }

        /// <summary>
        /// Agrega una auditoría de liga a la base de datos.
        /// </summary>
        public async Task AddAuditAsync(LeagueAudit audit)
        {
            _context.LeagueAudits.Add(audit);
            await _context.SaveChangesAsync();
        }

    }
}
