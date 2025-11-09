using System.ComponentModel.DataAnnotations;

namespace NFLFantasy.Api.DTO
{
    public class NflPlayerCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int PositionId { get; set; }

        [Required]
        public int NflTeamId { get; set; }

        [Required]
        public Microsoft.AspNetCore.Http.IFormFile Image { get; set; } = null!;
    }
}
