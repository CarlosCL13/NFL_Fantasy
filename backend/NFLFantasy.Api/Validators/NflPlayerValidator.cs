using System.Linq;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Data;
using NFLFantasy.Api.Models;
using NFLFantasy.Api.DataAccessLayer.Repositories;

namespace NFLFantasy.Api.Validators
{
    public class NflPlayerValidator
    {
        private readonly FantasyContext _context;
        public NflPlayerValidator(FantasyContext context)
        {
            _context = context;
        }

        public (bool IsValid, string? Error) ValidateCreate(NflPlayerCreateDto dto, INflPlayerRepository repository, bool requireImage = true)
        {
            // Validar campos requeridos (ya lo hace el modelo, pero por si acaso)
            if (string.IsNullOrWhiteSpace(dto.Name) || dto.PositionId <= 0 || (requireImage && dto.Image == null))
                return (false, "Todos los campos son obligatorios.");

            if (requireImage && dto.Image != null)
            {
                // Validar extensión de la imagen
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = System.IO.Path.GetExtension(dto.Image.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                    return (false, "Formato de imagen no permitido. Solo se permiten .jpg, .jpeg y .png");

                // Validar tamaño de la imagen (por ejemplo, máximo 2MB)
                const long maxFileSize = 2 * 1024 * 1024; // 2MB
                if (dto.Image.Length > maxFileSize)
                    return (false, "La imagen es demasiado grande. El tamaño máximo permitido es 2MB.");
            }

            // Validar existencia de equipo NFL
            if (!repository.NflTeamExists(dto.NflTeamId))
            {
                return (false, "El equipo NFL seleccionado no existe.");
            }

            // Validar jugador duplicado    
            if (repository.PlayerExists(dto.Name, dto.NflTeamId))
            {
                return (false, "Ya existe un jugador con ese nombre en el mismo equipo NFL.");
            }

            // Validar existencia de posición
            if (!repository.PositionExists(dto.PositionId))
            {
                return (false, "La posición seleccionada no existe.");
            }

            return (true, null);
        }
    }
}
