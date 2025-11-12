using System.Threading.Tasks;
using NFLFantasy.Api.Data;
using NFLFantasy.Api.Models;

namespace NFLFantasy.Api.Repositories
{
    public class NflPlayerRepository
    {
        private readonly FantasyContext _context;
        public NflPlayerRepository(FantasyContext context)
        {
            _context = context;
        }
        
        // Método para agregar un nuevo jugador NFL
        public async Task AddAsync(NflPlayer player)
        {
            _context.NflPlayers.Add(player);
            await _context.SaveChangesAsync();
        }

        // Métodos para validar existencia del equipo NFL
        public bool NflTeamExists(int nflTeamId)
        {
            return _context.NflTeams.Any(t => t.NflTeamId == nflTeamId);
        }
        
        // Método para validar existencia de posición
        public bool PositionExists(int positionId)
        {
            return _context.Positions.Any(p => p.PositionId == positionId);
        }

        // Método para validar existencia de jugador duplicado
        public bool PlayerExists(string name, int nflTeamId)
        {
            return _context.NflPlayers.Any(p => p.Name == name && p.NflTeamId == nflTeamId);
        }
    }
}
