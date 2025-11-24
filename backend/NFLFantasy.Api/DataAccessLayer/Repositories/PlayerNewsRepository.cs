using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NFLFantasy.Api.Models;
using NFLFantasy.Api.Data;

namespace NFLFantasy.Api.DataAccessLayer.Repositories
{
    public class PlayerNewsRepository
    {
        private readonly FantasyContext _context;
        public PlayerNewsRepository(FantasyContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PlayerNews news)
        {
            _context.PlayerNews.Add(news);
            await _context.SaveChangesAsync();
        }

        public async Task<List<PlayerNews>> GetByPlayerAsync(int playerId)
        {
            return await _context.PlayerNews
                .Where(n => n.PlayerId == playerId)
                .OrderByDescending(n => n.FechaCreacion)
                .ToListAsync();
        }
    }
}
