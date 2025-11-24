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
                return (false, AppConstants.ErrorMissingPlayerFields);

            if (requireImage && dto.Image != null)
            {
                // Validar extensión de la imagen
                var extension = Path.GetExtension(dto.Image.FileName).ToLowerInvariant();
                if (!AppConstants.AllowedImageExtensions.Contains(extension))
                    return (false, AppConstants.ErrorInvalidImageFormat);

                // Validar tamaño de la imagen
                if (dto.Image.Length > AppConstants.MaxImageFileSize)
                    return (false, AppConstants.ErrorImageTooLarge);
            }

            // Validar existencia de equipo NFL
            if (!repository.NflTeamExists(dto.NflTeamId))
            {
                return (false, AppConstants.ErrorNflTeamNotFound);
            }

            // Validar jugador duplicado    
            if (repository.PlayerExists(dto.Name, dto.NflTeamId))
            {
                return (false, AppConstants.ErrorPlayerAlreadyExists);
            }

            // Validar existencia de posición
            if (!repository.PositionExists(dto.PositionId))
            {
                return (false, AppConstants.ErrorPositionNotFound);
            }

            return (true, null);
        }
    }
}
