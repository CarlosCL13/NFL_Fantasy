using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NFLFantasy.Api.Models
{
    /// <summary>
    /// Representa un jugador de la NFL en la plataforma.
    /// </summary>
    public class NflPlayer
    {
        [Key]
        public int NflPlayerId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [ForeignKey("Position")]
        public int PositionId { get; set; }
        public Position Position { get; set; } = null!;

        [Required]
        [ForeignKey("NflTeam")]
        public int NflTeamId { get; set; }
        public NflTeam NflTeam { get; set; } = null!;

        [Required]
        [StringLength(200)]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string ThumbnailUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
