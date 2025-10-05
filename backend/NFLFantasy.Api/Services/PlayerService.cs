using NFLFantasy.Api.Models;
using NFLFantasy.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace NFLFantasy.Api.Services
{
    public class PlayerService
    {
        private readonly NFLFantasyDbContext _context;
        public PlayerService(NFLFantasyDbContext context)
        {
            _context = context;
        }

        public async Task<List<Player>> GetAllAsync() => await _context.Players.ToListAsync();
        public async Task<Player?> GetByIdAsync(int id) => await _context.Players.FindAsync(id);
        public async Task<Player> AddAsync(Player player)
        {
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
            return player;
        }
        public async Task<bool> UpdateAsync(int id, Player input)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null) return false;
            player.Name = input.Name;
            player.Position = input.Position;
            player.Team = input.Team;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null) return false;
            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
