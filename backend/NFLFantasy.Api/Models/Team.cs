using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NFLFantasy.Api.Models
{
    public class Team
    {
        public int TeamId { get; set; }

        [Required, StringLength(50)]
        public string TeamName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relación con User
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
