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
        public DbSet<Role> Roles { get; set; }
        public DbSet<LeagueAudit> LeagueAudits { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Scoring> Scorings { get; set; }
        public DbSet<DefaultPosition> DefaultPositions { get; set; }
        public DbSet<DefaultScoring> DefaultScorings { get; set; }

        public DbSet<NflPlayer> NflPlayers { get; set; }

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

            // Relación User-Role: un usuario tiene un rol, un rol puede tener muchos usuarios
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict); // Sin cascada para evitar múltiples rutas

            // Relación Team-User: un usuario puede tener varios equipos, pero sin cascada en el borrado
            modelBuilder.Entity<Team>()
                .HasOne(t => t.User)
                .WithMany(u => u.Teams)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Sin cascada para evitar múltiples rutas

            // Configuración explícita de la tabla y clave primaria para Role
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<Role>().HasKey(r => r.RoleId);

            // Relaciones para DefaultPosition
            modelBuilder.Entity<DefaultPosition>()
                .HasOne(dp => dp.League)
                .WithMany()
                .HasForeignKey(dp => dp.LeagueId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DefaultPosition>()
                .HasOne(dp => dp.Position)
                .WithMany()
                .HasForeignKey(dp => dp.PositionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relaciones para DefaultScoring
            modelBuilder.Entity<DefaultScoring>()
                .HasOne(ds => ds.League)
                .WithMany()
                .HasForeignKey(ds => ds.LeagueId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DefaultScoring>()
                .HasOne(ds => ds.Scoring)
                .WithMany()
                .HasForeignKey(ds => ds.ScoringId)
                .OnDelete(DeleteBehavior.Cascade);

             // Relación NflTeam-NflPlayer: un equipo tiene muchos jugadores, un jugador pertenece a un equipo
            modelBuilder.Entity<NflPlayer>()
                .HasOne(p => p.NflTeam)
                .WithMany()
                .HasForeignKey(p => p.NflTeamId)
                .OnDelete(DeleteBehavior.Restrict);    
        }
    }
}
