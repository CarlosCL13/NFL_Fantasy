using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NFLFantasy.Api.Models
{
    /// <summary>
    /// Representa un equipo dentro de una liga de fantasía NFL.
    /// </summary>
    public class Team
    {
        /// <summary>
        /// Alias único del equipo dentro de la liga.
        /// </summary>
        [Required, StringLength(30)]
        public string Alias { get; set; } = string.Empty;

        /// <summary>
        /// Identificador único del equipo.
        /// </summary>
        public int TeamId { get; set; }

        /// <summary>
        /// Nombre del equipo.
        /// </summary>
        [Required, StringLength(50)]
        public string TeamName { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de creación del equipo (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Relación con User.
        /// </summary>
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User? User { get; set; }

        /// <summary>
        /// Relación con League.
        /// </summary>
        [ForeignKey("League")]
        public int LeagueId { get; set; }
        public League? League { get; set; }
    }
}
