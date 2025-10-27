using System.ComponentModel.DataAnnotations;

namespace NFLFantasy.Api.DTO
{
    /// <summary>
    /// DTO para unirse a una liga con contraseña y datos de equipo.
    /// </summary>
    public class JoinLeagueDto
    {
        /// <summary>Id de la liga a la que se desea unir.</summary>
        [Required]
        public int LeagueId { get; set; }

        /// <summary>Contraseña de la liga.</summary>
        [Required]
        [StringLength(50)]
        public string Password { get; set; } = string.Empty;

        /// <summary>Alias único dentro de la liga.</summary>
        [Required]
        [StringLength(30)]
        public string Alias { get; set; } = string.Empty;

        /// <summary>Nombre de equipo único dentro de la liga.</summary>
        [Required]
        [StringLength(50)]
        public string TeamName { get; set; } = string.Empty;
    }
}
