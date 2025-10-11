using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Models;
using NFLFantasy.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace NFLFantasy.Api.Services
{
    /// <summary>
    /// Servicio para la gestión manual de equipos NFL por el administrador.
    /// </summary>
    public class NflTeamService
    {
        private readonly FantasyContext _context;
        public NflTeamService(FantasyContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Crea un nuevo equipo NFL si el nombre es único y los datos son válidos.
        /// </summary>
        /// <param name="dto">DTO con los datos del equipo.</param>
        /// <returns>Tupla con éxito, mensaje de error y el equipo creado.</returns>
        public async Task<(bool Success, string? Error, NflTeam? Team)> CreateNflTeamAsync(string name, string city, string imageFileName)
        {
            if (await _context.NflTeams.AnyAsync(t => t.Name == name))
                return (false, "El nombre del equipo ya existe.", null);

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(imageFileName))
                return (false, "Todos los campos son requeridos.", null);

            // Generar thumbnail automáticamente (ejemplo simple)
            var thumbnail = $"thumb_{Guid.NewGuid()}.png";

            var team = new NflTeam
            {
                Name = name,
                City = city,
                Image = imageFileName,
                Thumbnail = thumbnail,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.NflTeams.Add(team);
            await _context.SaveChangesAsync();
            return (true, null, team);
        }
    }
}
