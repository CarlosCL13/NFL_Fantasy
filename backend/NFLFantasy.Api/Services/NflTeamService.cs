using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Models;
using NFLFantasy.Api.Data;
using Microsoft.EntityFrameworkCore;
using NFLFantasy.Api;
using NFLFantasy.Api.DataAccessLayer.Repositories;
using NFLFantasy.Api.DataAccessLayer.StorageManagement;
using NFLFantasy.Api.Validators;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

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
        private readonly NFLFantasy.Api.DataAccessLayer.Repositories.NflTeamRepository _repository;

        //Referencia al manejador de directorios
        private readonly DirectoryManager _directoryManager;

        //Referencia al manejador de imágenes
        private readonly ImageStorageHandler _imageStorageHandler;
        
        /// <summary>
        /// Constructor del servicio NflTeamService.
        /// </summary>
        public NflTeamService(FantasyContext context, DirectoryManager directoryManager, ImageStorageHandler imageStorageHandler)
        {
            _context = context;
            _repository = new NFLFantasy.Api.DataAccessLayer.Repositories.NflTeamRepository(context);
            _directoryManager = directoryManager;
            _imageStorageHandler = imageStorageHandler;
        }

        /// <summary>
        /// Crea un nuevo equipo NFL si el nombre es único y los datos son válidos.
        /// </summary>
        /// <param name="dto">DTO con los datos del equipo.</param>
        /// <returns>Tupla con éxito, mensaje de error y el equipo creado.</returns>
        public async Task<(bool Success, string? Error, NflTeam? Team)> CreateNflTeamAsync(CreateNflTeamDto dto)
        {
            // Validar datos y archivo
            var (isValid, error) = await NflTeamValidator.ValidateCreateNflTeamAsync(dto, _repository);
            if (!isValid)
                return (false, error, null);

            // Guardar imagen y thumbnail
            var (imageFileName, thumbnailFileName, imageError) = await SaveImageAndThumbnailAsync(dto.Image);
            if (imageError != null)
                return (false, imageError, null);

            var team = new NflTeam
            {
                Name = dto.Name,
                City = dto.City,
                Image = imageFileName!,
                Thumbnail = thumbnailFileName!,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            try
            {
                await _repository.AddNflTeamAsync(team);
                return (true, null, team);
            }
            catch
            {
                // Si falla, elimina archivos huérfanos
                DeleteFileIfExists(imageFileName);
                DeleteFileIfExists(thumbnailFileName);
                return (false, "Error al guardar en base de datos.", null);
            }
        }

        private async Task<(string? imageFileName, string? thumbnailFileName, string? error)> SaveImageAndThumbnailAsync(Microsoft.AspNetCore.Http.IFormFile image)
        {
            try
            {
                var uploadsFolder = _directoryManager.GetNflTeamsImagesPath();
                var (imageFileName, thumbnailFileName) = await _imageStorageHandler.ProcessImageAsync(image, uploadsFolder);
                return (imageFileName, thumbnailFileName, null);
            }
            catch
            {
                return (null, null, "Error al guardar la imagen o el thumbnail.");
            }
        }

        private void DeleteFileIfExists(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return;
            var uploadsFolder = _directoryManager.GetNflTeamsImagesPath();
            var filePath = Path.Combine(uploadsFolder, fileName);
            if (System.IO.File.Exists(filePath))
            {
                try { System.IO.File.Delete(filePath); } catch { }
            }
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
