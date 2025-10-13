using System.ComponentModel.DataAnnotations;

namespace NFLFantasy.Api.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required, StringLength(50, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(50), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Alias { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Role { get; set; } = "manager";

        public string Status { get; set; } = "active";

        public string ProfileImage { get; set; } = "default.png";
        
        public string Language { get; set; } = "en";

        public int FailedLoginAttempts { get; set; } = 0;
        public DateTime? LastFailedLogin { get; set; }

        public ICollection<Team>? Teams { get; set; }

        }
}