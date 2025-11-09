using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Models;
using NFLFantasy.Api.Data;
using Microsoft.EntityFrameworkCore;
using NFLFantasy.Api;
using NFLFantasy.Api.Repositories;
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
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), AppConstants.NflTeamsImageFolder.Replace("/", Path.DirectorySeparatorChar.ToString()));
                Directory.CreateDirectory(uploadsFolder);

                var imageFileName = $"{Guid.NewGuid()}_{image.FileName}";
                var imagePath = Path.Combine(uploadsFolder, imageFileName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // Generar thumbnail
                var thumbnailFileName = $"thumb_{Guid.NewGuid()}.png";
                var thumbnailPath = Path.Combine(uploadsFolder, thumbnailFileName);

                using (var img = SixLabors.ImageSharp.Image.Load(imagePath))
                {
                    img.Mutate(x => x.Resize(100, 100));
                    img.Save(thumbnailPath, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
                }

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
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), AppConstants.NflTeamsImageFolder.Replace("/", Path.DirectorySeparatorChar.ToString()));
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
