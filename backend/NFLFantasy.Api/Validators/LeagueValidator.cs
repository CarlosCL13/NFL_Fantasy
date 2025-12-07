using System.Text.RegularExpressions;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NFLFantasy.Api.Utils;
using NFLFantasy.Api.Models;
using NFLFantasy.Api.DataAccessLayer.Repositories;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Data;

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
            CreateLeagueDto dto,
            FantasyContext context,
            ILeagueRepository leagueRepository)
        {
            // Validar nombre de liga
            if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Length > 100){
                return (false, AppConstants.ErrorInvalidLeagueName);
            }

            // Validar nombre único de liga
            if (await leagueRepository.LeagueNameExistsAsync(dto.Name)){
                return (false, AppConstants.ErrorLeagueNameExists);
            }

            // Validar cantidad de equipos
            if (!IsValidTeamCount(dto.MaxTeams)){
                return (false, AppConstants.ErrorInvalidTeamCount);
            }

            // Validar contraseña
            if (!IsValidPassword(dto.Password)){
                return (false, AppConstants.ErrorInvalidLeaguePassword);
            }

            // Validar existencia de temporada actual
            var season = await leagueRepository.GetCurrentSeasonAsync();
            if (season == null){
                return (false, AppConstants.ErrorNoActiveSeason);
            }

            // Validar existencia de alias del comisionado
            if (await leagueRepository.AliasExistsAsync(dto.CommissionerAlias)){
                return (false, AppConstants.ErrorTeamAliasExists);
            }

            return (true, null);
        }

        /// <summary>
        /// Validaciones centralizadas para unirse a una liga.
        /// </summary>
        public static async Task<(bool IsValid, string? Error, League? League)> ValidateJoinLeagueAsync(
            int userId,
            JoinLeagueDto dto,
            FantasyContext context)
        {
            // Validar existencia de la liga y contraseña
            var league = await context.Leagues
                .Include(l => l.Teams)
                .FirstOrDefaultAsync(l => l.LeagueId == dto.LeagueId);

            // Valida que la liga exista
            if (league == null)
                return (false, AppConstants.ErrorLeagueNotFound, null);

            // Valida que la liga esté activa
            if (!league.IsActive)
                return (false, AppConstants.ErrorLeagueInactive, null);

            // Valida la contraseña
            if (!PasswordHelper.VerifyPassword(dto.Password, league.PasswordHash))
                return (false, AppConstants.ErrorIncorrectPassword, null);

            // Valida que haya cupos
            if (league.Teams.Count >= league.MaxTeams)
                return (false, AppConstants.ErrorLeagueFull, null);

            // Valida que el alias y nombre de equipo sean únicos en la liga
            if (league.Teams.Any(t => t.Alias == dto.Alias))
                return (false, AppConstants.ErrorAliasExistsInLeague, null);

            if (league.Teams.Any(t => t.TeamName == dto.TeamName))
                return (false, AppConstants.ErrorTeamNameExistsInLeague, null);

            // Valida que el usuario no pertenezca ya a la liga
            if (league.Teams.Any(t => t.UserId == userId))
                return (false, AppConstants.ErrorUserAlreadyInLeague, null);

            return (true, null, league);

        }
    }
}
