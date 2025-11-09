using System.ComponentModel.DataAnnotations;

namespace NFLFantasy.Api.DTO
{
    public class NflPlayerBulkDto
    {
        /// <summary>
        /// Nombre completo del jugador NFL.
        /// </summary>
        [Required]
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
        /// Ruta local de la imagen del jugador.
        /// </summary>
        [Required]
        public string ImagePath { get; set; } = string.Empty; // Ruta local de la imagen
    }
}
