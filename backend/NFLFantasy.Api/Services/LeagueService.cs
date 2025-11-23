using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NFLFantasy.Api.Data;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Models;
using BCrypt.Net;

using NFLFantasy.Api.Utils;
namespace NFLFantasy.Api.Services
{
    /// <summary>
    /// Servicio para la gestión de ligas.
    /// </summary>
    public class LeagueService
    {
        // Referencia al contexto de la base de datos
        private readonly FantasyContext _context;

        // Referencia al repositorio de ligas
        private readonly NFLFantasy.Api.DataAccessLayer.Repositories.LeagueRepository _leagueRepository;

        /// <summary>
        /// Constructor del servicio LeagueService.
        /// </summary>
        public LeagueService(FantasyContext context)
        {
            _context = context;
            _leagueRepository = new NFLFantasy.Api.DataAccessLayer.Repositories.LeagueRepository(context);
        }

        /// <summary>
        /// Busca ligas en la temporada actual por nombre y/o estado.
        /// </summary>
        /// <param name="dto">DTO con filtros de búsqueda.</param>
        /// <returns>Lista de ligas que cumplen los filtros.</returns>
        
        public async Task<List<League>> SearchLeaguesAsync(SearchLeagueDto dto)
        {
            var currentSeason = await GetCurrentSeasonOrThrowAsync();
            var query = BuildLeagueSearchQuery(dto, currentSeason.SeasonId);
            return await query.ToListAsync();
        }

        /// <summary>
        /// Permite a un usuario unirse a una liga con contraseña y validaciones.
        /// </summary>
        /// <param name="userId">Id del usuario que se une.</param>
        /// <param name="dto">DTO con datos de unión: id de liga, contraseña, alias y nombre de equipo.</param>
        /// <returns>Tupla con éxito y mensaje de error si aplica.</returns>
        public async Task<(bool Success, string? Error)> JoinLeagueAsync(int userId, JoinLeagueDto dto)
        {
            // Validaciones centralizadas en LeagueValidator
            var (isValid, error, league) = await NFLFantasy.Api.Validators.LeagueValidator.ValidateJoinLeagueAsync(userId, dto, _context);
            if (!isValid)
                return (false, error);

            // Crear equipo y registrar auditoría
            var team = new Team
            {
                TeamName = dto.TeamName,
                Alias = dto.Alias,
                UserId = userId,
                LeagueId = league!.LeagueId,
                CreatedAt = DateTime.UtcNow
            };

            // Usar transacción para atomicidad
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _leagueRepository.AddTeamAsync(team);

                DecrementRemainingSpots(league);

                var audit = new LeagueAudit
                {
                    UserId = userId,
                    LeagueId = league.LeagueId,
                    Action = "Join",
                    Timestamp = DateTime.UtcNow
                };

                await _leagueRepository.AddAuditAsync(audit);
                
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            return (true, null);
        }

        /// <summary>
        /// Crea una nueva liga con los datos proporcionados.
        /// </summary>
        /// <param name="dto">DTO con los datos de la liga.</param>
        /// <param name="userId">ID del usuario que crea la liga.</param>
        /// <returns>Tupla con éxito, mensaje de error, liga creada y espacios restantes.</returns>
        /// <remarks>
        /// Este método crea una nueva liga en el sistema y asigna al usuario especificado como comisionado principal.
        /// </remarks>
        public async Task<(bool Success, string? Error, League? League, int? RemainingSpots)> CreateLeagueAsync(CreateLeagueDto dto, int userId)
        {

            // Validaciones centralizadas en LeagueValidator
            var (isValid, error) = await NFLFantasy.Api.Validators.LeagueValidator.ValidateCreateLeagueAsync(dto, _context, _leagueRepository);
            if (!isValid)
                return (false, error, null, null);

            // Buscar temporada actual (ya validado que existe)
            var season = await _leagueRepository.GetCurrentSeasonAsync();

            // Hash de la contraseña
            var passwordHash = PasswordHelper.HashPassword(dto.Password); // Se mantiene solo esta línea

            // Crear liga
            var league = new League
            {
                Name = dto.Name,
                Description = dto.Description,
                MaxTeams = dto.MaxTeams,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow,
                Status = "Pre-Draft",
                SeasonId = season!.SeasonId,
                CommissionerId = userId,
                PlayoffType = dto.PlayoffType,
                AllowDecimalPoints = true,
                TradeDeadlineActive = false,
                MaxTradesPerTeam = null,
                MaxFreeAgentsPerTeam = null,
                RemainingSpots = dto.MaxTeams - 1 // Se descuenta el equipo del comisionado
            };

            // Usar transacción para atomicidad
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _leagueRepository.AddLeagueAsync(league);
                await AddDefaultPositionsAsync(league.LeagueId);
                await AddDefaultScoringsAsync(league.LeagueId);
                await _context.SaveChangesAsync();

                // Crear equipo del comisionado
                var team = new Team
                {
                    TeamName = dto.CommissionerTeamName,
                    Alias = dto.CommissionerAlias,
                    UserId = userId,
                    LeagueId = league.LeagueId,
                    CreatedAt = DateTime.UtcNow
                };
                await _leagueRepository.AddTeamAsync(team);

                // Confirmar transacción
                await transaction.CommitAsync();
            }

            // Manejo de errores
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, ex.Message, null, null);
            }

            // Calcular cupos restantes
            var remainingSpots = CalculateRemainingSpots(league.MaxTeams);

            // Retornar resultado exitoso
            return (true, null, league, remainingSpots);
        }


        /// <summary>
        /// Agrega posiciones por defecto a una liga
        /// </summary>
        private async Task AddDefaultPositionsAsync(int leagueId)
        {
            var posicionesPorDefecto = new List<(string abrev, int cantidad)>
            {
                ("QB", 1),
                ("RB", 2),
                ("WR", 2),
                ("TE", 1),
                ("K", 1),
                ("DEF", 1),
                ("RB/WR", 1),
                ("BN", 6),
                ("IR", 3)
            };
            foreach (var (abrev, cantidad) in posicionesPorDefecto)
            {
                var posicion = await _context.Positions.FirstOrDefaultAsync(p => p.Abbreviation == abrev);
                if (posicion != null)
                {
                    _context.DefaultPositions.Add(new DefaultPosition
                    {
                        LeagueId = leagueId,
                        PositionId = posicion.PositionId,
                        Quantity = cantidad
                    });
                }
            }
        }

        /// <summary>
        /// Agrega reglas de puntuación por defecto a una liga
        /// </summary>
        /// <param name="leagueId"></param>
        /// <returns></returns>
        private async Task AddDefaultScoringsAsync(int leagueId)
        {
            var scoringPorDefecto = new List<(string nombre, double valor)>
            {
                ("Passing Yards", 1.0/25),
                ("Passing Touchdowns", 4),
                ("Interceptions Thrown", -2),
                ("Rushing Yards", 1.0/10),
                ("Receptions", 1),
                ("Receiving Yards", 1.0/10),
                ("Rush/Recv Touchdowns", 6),
                ("Sacks", 1),
                ("Interceptions", 2),
                ("Fumbles Recovered", 2),
                ("Safeties", 2),
                ("Touchdowns", 6),
                ("Team Def 2-point Return", 2),
                ("PAT Made", 1),
                ("FG Made 0-50", 3),
                ("FG Made 50+", 5),
                ("Points Allowed <=10", 5),
                ("Points Allowed <=20", 2),
                ("Points Allowed <=30", 0),
                ("Points Allowed >30", -2)
            };
            foreach (var (nombre, valor) in scoringPorDefecto)
            {
                var regla = await _context.Scorings.FirstOrDefaultAsync(s => s.Name == nombre);
                if (regla != null)
                {
                    _context.DefaultScorings.Add(new DefaultScoring
                    {
                        LeagueId = leagueId,
                        ScoringId = regla.ScoringId,
                        Value = valor
                    });
                }
            }
        }

        /// <summary>
        /// Calcula cupos restantes tras crear el equipo del comisionado
        /// </summary>
        private int CalculateRemainingSpots(int maxTeams) => maxTeams - 1;

        /// <summary>
        /// Decrementa los cupos restantes en una liga al unirse un equipo
        /// </summary>
        private void DecrementRemainingSpots(League league)
        {
            if (league.RemainingSpots > 0)
                league.RemainingSpots--;
        }

        /// <summary>
        /// Obtiene la temporada actual o lanza una excepción si no existe.
        /// </summary>
        private async Task<Season> GetCurrentSeasonOrThrowAsync()
        {
            var season = await _context.Seasons.FirstOrDefaultAsync(s => s.IsCurrent);
            if (season == null)
                throw new InvalidOperationException("No se encontró una temporada actual.");
            return season;
        }

        /// <summary>
        /// Construye la consulta de búsqueda de ligas según los filtros proporcionados.
        /// </summary>
        private IQueryable<League> BuildLeagueSearchQuery(SearchLeagueDto dto, int seasonId)
        {
            var query = _context.Leagues.Where(l => l.SeasonId == seasonId);

            if (!string.IsNullOrWhiteSpace(dto.Name))
                query = query.Where(l => l.Name.Contains(dto.Name));

            if (dto.IsActive.HasValue)
                query = query.Where(l => l.IsActive == dto.IsActive.Value);

            return query;
        }
    }
}
