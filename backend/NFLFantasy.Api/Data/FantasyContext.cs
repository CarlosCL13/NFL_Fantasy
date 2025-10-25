using Microsoft.EntityFrameworkCore;
using NFLFantasy.Api.Models;

namespace NFLFantasy.Api.Data
{
    public class FantasyContext : DbContext
    {
        public FantasyContext(DbContextOptions<FantasyContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<NflTeam> NflTeams { get; set; }

    public DbSet<Season> Seasons { get; set; }
    public DbSet<Week> Weeks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Team>()
                .Property(t => t.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<NflTeam>()
                .Property(t => t.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Season>()
                .Property(s => s.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}
