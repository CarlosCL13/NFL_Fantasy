using Microsoft.EntityFrameworkCore;
using NFLFantasy.Api.Models;

namespace NFLFantasy.Api.Data
{
    /// <summary>
    /// Contexto de la base de datos para la aplicación NFL Fantasy.
    /// </summary>
    public class FantasyContext : DbContext
    {
        /// <summary>
        /// Constructor del contexto de la base de datos.
        /// </summary>
        public FantasyContext(DbContextOptions<FantasyContext> options) : base(options) { }

        /// <summary>
        /// Conjuntos de entidades en el contexto.
        /// </summary>
    public DbSet<User> Users { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<NflTeam> NflTeams { get; set; }
    public DbSet<League> Leagues { get; set; }
    public DbSet<Season> Seasons { get; set; }
    public DbSet<Week> Weeks { get; set; }
    public DbSet<LeagueAudit> LeagueAudits { get; set; }

        /// <summary>
        /// Configuraciones adicionales del modelo.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Índice único en el correo electrónico del usuario
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configuración de valores por defecto para CreatedAt
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
