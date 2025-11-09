using System.ComponentModel.DataAnnotations;

namespace NFLFantasy.Api.DTO
{
    public class NflPlayerCreateDto
    {
        /// <summary>
        /// Nombre completo del jugador NFL.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Identificador de la posición del jugador.
        /// </summary>
        [Required]
        public int PositionId { get; set; }

        /// <summary>
        /// Identificador del equipo NFL al que pertenece el jugador.
        /// </summary>
        [Required]
        public int NflTeamId { get; set; }

        /// <summary>
        /// Imagen del jugador (archivo).
        /// </summary>
        [Required]
        public Microsoft.AspNetCore.Http.IFormFile Image { get; set; } = null!;
    }
}
