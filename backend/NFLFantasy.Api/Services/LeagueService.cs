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
