using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NFLFantasy.Api.Models;
using NFLFantasy.Api.Data;

namespace NFLFantasy.Api.DataAccessLayer.Repositories
{
    public interface IPlayerNewsRepository
    {
        Task AddAsync(PlayerNews news);
        Task<List<PlayerNews>> GetByPlayerAsync(int playerId);
    }

    public class PlayerNewsRepository : IPlayerNewsRepository
    {
        private readonly FantasyContext _context;
        public PlayerNewsRepository(FantasyContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Agrega una noticia de jugador a la base de datos.
        /// </summary>
        public async Task AddAsync(PlayerNews news)
        {
            _context.PlayerNews.Add(news);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtiene las noticias de un jugador específico en orden cronológico inverso.
        /// </summary>
        public async Task<List<PlayerNews>> GetByPlayerAsync(int playerId)
        {
            return await _context.PlayerNews
                .Where(n => n.PlayerId == playerId)
                .OrderByDescending(n => n.FechaCreacion)
                .ToListAsync();
        }
    }
}
