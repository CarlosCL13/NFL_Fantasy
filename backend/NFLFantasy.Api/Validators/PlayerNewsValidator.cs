using System.Collections.Generic;
using NFLFantasy.Api.DTO;

namespace NFLFantasy.Api.Validators
{
    public static class PlayerNewsValidator
    {
        private static readonly HashSet<string> ValidDesignaciones = new HashSet<string> { "O", "D", "Q", "P", "FP", "IR", "PUP", "SUS" };

        public static List<string> Validate(CreatePlayerNewsDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Texto) || dto.Texto.Length < 10 || dto.Texto.Length > 300)
                errors.Add("El texto debe tener entre 10 y 300 caracteres.");

            if (dto.IsLesion)
            {
                if (string.IsNullOrWhiteSpace(dto.Resumen) || dto.Resumen.Length > 30)
                    errors.Add("El resumen de lesión es requerido y debe tener hasta 30 caracteres.");
                if (string.IsNullOrWhiteSpace(dto.Designacion) || !ValidDesignaciones.Contains(dto.Designacion))
                    errors.Add("La designación de lesión es requerida y debe ser una de: O, D, Q, P, FP, IR, PUP, SUS.");
            }

            return errors;
        }
    }
}
