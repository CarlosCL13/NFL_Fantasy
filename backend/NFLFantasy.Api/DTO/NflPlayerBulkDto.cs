using System.ComponentModel.DataAnnotations;

namespace NFLFantasy.Api.DTO
{
    public class NflPlayerBulkDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public int PositionId { get; set; }
        [Required]
        public int NflTeamId { get; set; }
        [Required]
        public string ImagePath { get; set; } = string.Empty; // Ruta local de la imagen
    }
}
