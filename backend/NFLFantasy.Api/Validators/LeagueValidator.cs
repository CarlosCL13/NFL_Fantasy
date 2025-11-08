using System.Text.RegularExpressions;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NFLFantasy.Api.Utils;
using NFLFantasy.Api.Models;

namespace NFLFantasy.Api.Validators
{
    public static class LeagueValidator
    {

        /// <summary>
        /// Valida si la cantidad de equipos es válida.
        /// </summary>
        public static bool IsValidTeamCount(int count)
        {
            return new[] { 4, 6, 8, 10, 12, 14, 16, 18, 20 }.Contains(count);
        }

        /// <summary>
        /// Valida si la contraseña cumple con el formato requerido.
        /// </summary>
        public static bool IsValidPassword(string password)
        {
            return Regex.IsMatch(password, "^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).{8,12}$");
        }

        /// <summary>
        /// Validaciones centralizadas para la creación de liga.
        /// </summary>
        public static async Task<(bool IsValid, string? Error)> ValidateCreateLeagueAsync(
            NFLFantasy.Api.DTO.CreateLeagueDto dto,
            NFLFantasy.Api.Data.FantasyContext context)
        {
            // Validar nombre de liga
            if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Length > 100)
                return (false, "El nombre de la liga debe tener entre 1 y 100 caracteres.");

            // Validar nombre único de liga
            if (await context.Leagues.AnyAsync(l => l.Name == dto.Name))
                return (false, "Ya existe una liga con ese nombre.");

            // Validar cantidad de equipos
            if (!IsValidTeamCount(dto.MaxTeams))
                return (false, "La cantidad de equipos no es válida.");

            // Validar contraseña
            if (!IsValidPassword(dto.Password))
                return (false, "La contraseña no cumple el formato requerido.");

            // Validar existencia de temporada actual
            var season = await context.Seasons.FirstOrDefaultAsync(s => s.IsCurrent);
            if (season == null)
                return (false, "No hay una temporada actual activa.");

            // Validar existencia de alias del comisionado
            var aliasExists = await context.Teams.AnyAsync(t => t.Alias == dto.CommissionerAlias);
            if (aliasExists)
                return (false, "El alias del equipo ya existe en el sistema. Intente con un nombre de equipo diferente.");

            return (true, null);
        }

        /// <summary>
        /// Validaciones centralizadas para unirse a una liga.
        /// </summary>
        public static async Task<(bool IsValid, string? Error, League? League)> ValidateJoinLeagueAsync(
            int userId,
            NFLFantasy.Api.DTO.JoinLeagueDto dto,
            NFLFantasy.Api.Data.FantasyContext context)
        {
            // Validar existencia de la liga y contraseña
            var league = await context.Leagues
                .Include(l => l.Teams)
                .FirstOrDefaultAsync(l => l.LeagueId == dto.LeagueId);

            // Valida que la liga exista
            if (league == null)
                return (false, "La liga no existe.", null);

            // Valida que la liga esté activa
            if (!league.IsActive)
                return (false, "La liga no está activa.", null);

            // Valida la contraseña
            if (!PasswordHelper.VerifyPassword(dto.Password, league.PasswordHash))
                return (false, "La contraseña es incorrecta.", null);

            // Valida que haya cupos
            if (league.Teams.Count >= league.MaxTeams)
                return (false, "No hay cupos disponibles en la liga.", null);

            // Valida que el alias y nombre de equipo sean únicos en la liga
            if (league.Teams.Any(t => t.Alias == dto.Alias))
                return (false, "El alias ya existe en la liga. Elige otro.", null);

            if (league.Teams.Any(t => t.TeamName == dto.TeamName))
                return (false, "El nombre de equipo ya existe en la liga. Elige otro.", null);

            // Valida que el usuario no pertenezca ya a la liga
            if (league.Teams.Any(t => t.UserId == userId))
                return (false, "Ya perteneces a esta liga.", null);

            return (true, null, league);

        }
    }
}
