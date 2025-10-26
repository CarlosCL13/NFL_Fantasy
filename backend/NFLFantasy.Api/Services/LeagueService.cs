using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NFLFantasy.Api.Data;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Models;
using BCrypt.Net;

namespace NFLFantasy.Api.Services
{
    /// <summary>
    /// Servicio para la gestión de ligas.
    /// </summary>
    public class LeagueService
    {
        // Referencia al contexto de la base de datos
        private readonly FantasyContext _context;

        /// <summary>
        /// Constructor del servicio LeagueService.
        /// </summary>
        public LeagueService(FantasyContext context)
        {
            _context = context;
        }
            /// <summary>
            /// Busca ligas por nombre, temporada y estado.
            /// </summary>
            /// <param name="dto">DTO con filtros de búsqueda.</param>
            /// <returns>Lista de ligas que cumplen los filtros.</returns>
            
            public async Task<List<League>> SearchLeaguesAsync(SearchLeagueDto dto)
            {
                var query = _context.Leagues.AsQueryable();
                if (!string.IsNullOrWhiteSpace(dto.Name))
                    query = query.Where(l => l.Name.Contains(dto.Name));
                if (dto.SeasonId.HasValue)
                    query = query.Where(l => l.SeasonId == dto.SeasonId.Value);
                if (dto.IsActive.HasValue)
                    query = query.Where(l => l.IsActive == dto.IsActive.Value);
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
                var league = await _context.Leagues
                    .Include(l => l.Teams)
                    .FirstOrDefaultAsync(l => l.LeagueId == dto.LeagueId);
                if (league == null)
                    return (false, "La liga no existe.");
                if (!league.IsActive)
                    return (false, "La liga no está activa.");
                if (!BCrypt.Net.BCrypt.Verify(dto.Password, league.PasswordHash))
                    return (false, "Datos incorrectos."); // error genérico
                if (league.Teams.Count >= league.MaxTeams)
                    return (false, "No hay cupos disponibles en la liga.");
                if (league.Teams.Any(t => t.Alias == dto.Alias))
                    return (false, "El alias ya existe en la liga. Elige otro.");
                if (league.Teams.Any(t => t.TeamName == dto.TeamName))
                    return (false, "El nombre de equipo ya existe en la liga. Elige otro.");
                if (league.Teams.Any(t => t.UserId == userId))
                    return (false, "Ya perteneces a esta liga.");

                // Crear equipo y registrar auditoría
                var team = new Team
                {
                    TeamName = dto.TeamName,
                    Alias = dto.Alias,
                    UserId = userId,
                    LeagueId = league.LeagueId,
                    CreatedAt = DateTime.UtcNow
                };
                league.Teams.Add(team);
                // Registrar auditoría (simplificado)
                var audit = new LeagueAudit
                {
                    UserId = userId,
                    LeagueId = league.LeagueId,
                    Action = "Join",
                    Timestamp = DateTime.UtcNow
                };
                _context.LeagueAudits.Add(audit);
                await _context.SaveChangesAsync();
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
            // Validar nombre único
            if (await _context.Leagues.AnyAsync(l => l.Name == dto.Name))
                return (false, "Ya existe una liga con ese nombre.", null, null);

            // Validar cantidad de equipos permitida
            var allowedTeams = new[] { 4, 6, 8, 10, 12, 14, 16, 18, 20 };
            if (!allowedTeams.Contains(dto.MaxTeams))
                return (false, "La cantidad de equipos no es válida.", null, null);

            // Validar formato de contraseña
            if (!System.Text.RegularExpressions.Regex.IsMatch(dto.Password, "^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).{8,12}$"))
                return (false, "La contraseña no cumple el formato requerido.", null, null);

            // Buscar temporada actual
            var season = await _context.Seasons.FirstOrDefaultAsync(s => s.IsCurrent);
            if (season == null)
                return (false, "No hay una temporada actual activa.", null, null);

            // Hash de la contraseña
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Esquema de posiciones y puntuación por defecto (puedes ajustar el formato)
            var defaultPositions = "1 QB, 2 RB, 1 K, 1 DEF, 2 WR, 1 RB/WR, 1 TE, 6 BN, 3 IR";
            var defaultScoring = "PassingYards:1/25,PassingTD:4,IntThrown:-2,RushingYards:1/10,Receptions:1,ReceivingYards:1/10,RushRecvTD:6,Sacks:1,Interceptions:2,FumblesRecovered:2,Safeties:2,Touchdowns:6,TeamDef2ptReturn:2,PATMade:1,FG0-50:3,FG50+:5,PointsAllowed<=10:5,PointsAllowed<=20:2,PointsAllowed<=30:0,PointsAllowed>30:-2";

            // Crear liga
            var league = new League
            {
                Name = dto.Name,
                Description = dto.Description,
                MaxTeams = dto.MaxTeams,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow,
                Status = "Pre-Draft",
                SeasonId = season.SeasonId,
                CommissionerId = userId,
                PlayoffType = dto.PlayoffType,
                AllowDecimalPoints = true,
                DefaultPositions = defaultPositions,
                DefaultScoring = defaultScoring,
                TradeDeadlineActive = false,
                MaxTradesPerTeam = null,
                MaxFreeAgentsPerTeam = null
            };
            _context.Leagues.Add(league);
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

            // Guardar equipo en la base de datos
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            // Calcular cupos restantes
            var remainingSpots = league.MaxTeams - 1;

            // Devolver resultado
            return (true, null, league, remainingSpots);
        }
    }
}
