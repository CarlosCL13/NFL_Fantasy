using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NFLFantasy.Api.DTO
{
    public class CreateSeasonDto
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(1, 30)] // (Suponiendo máximo 30 semanas por temporada)
        public int WeeksCount { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsCurrent { get; set; } = false; //estado actual
    }
}
