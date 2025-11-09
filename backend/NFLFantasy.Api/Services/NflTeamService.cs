using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Models;
using NFLFantasy.Api.Data;
using Microsoft.EntityFrameworkCore;
using NFLFantasy.Api;
using NFLFantasy.Api.Repositories;
using NFLFantasy.Api.Validators;

namespace NFLFantasy.Api.Services
{
    /// <summary>
    /// Servicio para la gestión manual de equipos NFL por el administrador.
    /// </summary>
    public class NflTeamService
    {
        //Referencia al contexto de la base de datos
        private readonly FantasyContext _context;

        //Referencia al repositorio de equipos NFL
        private readonly NFLFantasy.Api.Repositories.NflTeamRepository _repository;
        
        /// <summary>
        /// Constructor del servicio NflTeamService.
        /// </summary>
        public NflTeamService(FantasyContext context)
        {
            _context = context;
            _repository = new NFLFantasy.Api.Repositories.NflTeamRepository(context);
        }

        /// <summary>
        /// Crea un nuevo equipo NFL si el nombre es único y los datos son válidos.
        /// </summary>
        /// <param name="dto">DTO con los datos del equipo.</param>
        /// <returns>Tupla con éxito, mensaje de error y el equipo creado.</returns>
        public async Task<(bool Success, string? Error, NflTeam? Team)> CreateNflTeamAsync(string name, string city, string imageFileName, string thumbnailFileName)
        {
            // Validaciones centralizadas en NflTeamValidator
            var (isValid, error) = await NflTeamValidator.ValidateCreateNflTeamAsync(name, city, imageFileName, thumbnailFileName, _repository);
            if (!isValid)
            {
                return (false, error, null);
            }

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
            await _repository.AddNflTeamAsync(team);

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
