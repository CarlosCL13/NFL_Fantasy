using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Models;
using NFLFantasy.Api.Data;
using Microsoft.EntityFrameworkCore;
using NFLFantasy.Api;

namespace NFLFantasy.Api.Services
{
    /// <summary>
    /// Servicio para la gestión manual de equipos NFL por el administrador.
    /// </summary>
    public class NflTeamService
    {
        //Referencia al contexto de la base de datos
        private readonly FantasyContext _context;
        
        /// <summary>
        /// Constructor del servicio NflTeamService.
        /// </summary>
        public NflTeamService(FantasyContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Crea un nuevo equipo NFL si el nombre es único y los datos son válidos.
        /// </summary>
        /// <param name="dto">DTO con los datos del equipo.</param>
        /// <returns>Tupla con éxito, mensaje de error y el equipo creado.</returns>
        public async Task<(bool Success, string? Error, NflTeam? Team)> CreateNflTeamAsync(string name, string city, string imageFileName, string thumbnailFileName)
        {

            // Validar nombre único
            if (await _context.NflTeams.AnyAsync(t => t.Name == name))
                return (false, AppConstants.ErrorNflTeamNameExists, null);

            // Validar campos obligatorios
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(imageFileName) || string.IsNullOrWhiteSpace(thumbnailFileName))
                return (false, AppConstants.ErrorMissingNflTeamFields, null);

            // Crear el equipo NFL
            var team = new NflTeam
            {
                Name = name,
                City = city,
                Image = imageFileName,
                Thumbnail = thumbnailFileName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // Guardar en la base de datos
            _context.NflTeams.Add(team);
            await _context.SaveChangesAsync();
            return (true, null, team);
        }


        /// <summary>
        /// Obtiene la lista de todos los equipos NFL creados.
        /// </summary>
        /// <returns>Lista de equipos NFL.</returns>
        public async Task<List<NflTeam>> GetAllNflTeamsAsync()
        {
            // Obtener todos los equipos NFL ordenados por nombre
            return await _context.NflTeams.OrderBy(t => t.Name).ToListAsync();
        }
    }
}
