using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NFLFantasy.Api.DTO
{
    public class CreateSeasonDto
    {
        /// <summary>
        /// Nombre de la temporada.
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Número de semanas en la temporada.
        /// </summary>
        [Required]
        [Range(1, 30)] // (Suponiendo máximo 30 semanas por temporada)
        public int WeeksCount { get; set; }

        /// <summary>
        /// Fecha de inicio de la temporada (UTC).
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }
        
        /// <summary>
        /// Fecha de finalización de la temporada (UTC).
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Indica si esta es la temporada actual.
        /// </summary>
        public bool IsCurrent { get; set; } = false; //estado actual
    }
}
