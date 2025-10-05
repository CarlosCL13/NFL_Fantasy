using Microsoft.EntityFrameworkCore;

namespace NFLFantasy.Api.Data
{
    public class NFLFantasyDbContext : DbContext
    {
        public NFLFantasyDbContext(DbContextOptions<NFLFantasyDbContext> options)
            : base(options)
        {
        }

    public DbSet<Models.Player> Players { get; set; }
    }
}
