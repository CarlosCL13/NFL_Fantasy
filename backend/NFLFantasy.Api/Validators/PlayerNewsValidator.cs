using System.Collections.Generic;
using NFLFantasy.Api.DTO;

namespace NFLFantasy.Api.Validators
{
    public static class PlayerNewsValidator
    {
        /// <summary>
        /// Valida el DTO para crear noticias de jugadores usando los IDs de designación de la base de datos.
        /// </summary>
        public static List<string> Validate(CreatePlayerNewsDto dto, NFLFantasy.Api.Data.FantasyContext context)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Texto) || dto.Texto.Length < 10 || dto.Texto.Length > 300)
            {
                errors.Add("El texto debe tener entre 10 y 300 caracteres.");
            }
                

            if (dto.IsLesion)
            {
                if (string.IsNullOrWhiteSpace(dto.Resumen) || dto.Resumen.Length > 30)
                    errors.Add("El resumen de lesión es requerido y debe tener hasta 30 caracteres.");

                var validIds = context.Designaciones.Select(d => d.Id).ToHashSet();
                if (!dto.DesignacionId.HasValue || !validIds.Contains(dto.DesignacionId.Value))
                    errors.Add("La designación de lesión es requerida y debe ser un ID válido.");
            }

            return errors;
        }
    }
}
