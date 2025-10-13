using System.ComponentModel.DataAnnotations;

namespace NFLFantasy.Api.Models
{
    /// <summary>
    /// Representa un equipo real de la NFL gestionado manualmente por el administrador.
    /// </summary>
    public class NflTeam
    {
        /// <summary>
        /// Identificador único del equipo NFL.
        /// </summary>
        public int NflTeamId { get; set; }

        /// <summary>
        /// Nombre oficial del equipo NFL.
        /// </summary>
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Ciudad base del equipo NFL.
        /// </summary>
        [Required, StringLength(100)]
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// URL o nombre de archivo de la imagen principal del equipo.
        /// </summary>
        [Required]
        public string Image { get; set; } = string.Empty;

        /// <summary>
        /// URL o nombre de archivo del thumbnail generado automáticamente.
        /// </summary>
        [Required]
        public string Thumbnail { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de creación del registro (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Indica si el equipo está activo (por defecto true).
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
