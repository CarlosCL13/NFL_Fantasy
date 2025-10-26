using System.ComponentModel.DataAnnotations;

namespace NFLFantasy.Api.DTO
{
    /// <summary>
    /// DTO para buscar ligas por nombre, temporada y estado.
    /// </summary>
    public class SearchLeagueDto
    {
        /// <summary>Nombre parcial o completo de la liga.</summary>
        [StringLength(100)]
        public string? Name { get; set; }

        /// <summary>Id de la temporada para filtrar.</summary>
        public int? SeasonId { get; set; }

        /// <summary>Estado de la liga (activa/inactiva).</summary>
        public bool? IsActive { get; set; }
    }
}
