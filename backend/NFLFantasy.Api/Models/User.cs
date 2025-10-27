using System.ComponentModel.DataAnnotations;

namespace NFLFantasy.Api.Models
{

    /// <summary>
    /// Representa un usuario en el sistema de la aplicación NFL Fantasy.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Identificador único del usuario.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        [Required, StringLength(50, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Correo electrónico del usuario.
        /// </summary>
        [Required, StringLength(50), EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Alias del usuario.
        /// </summary>
        [Required, StringLength(50)]
        public string Alias { get; set; } = string.Empty;

        /// <summary>
        /// Hash de la contraseña del usuario.
        /// </summary>
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de creación del usuario (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Rol del usuario.
        /// </summary>
        public string Role { get; set; } = "manager";

        /// <summary>
        /// Estado del usuario (activo, inactivo, suspendido).
        /// </summary>
        public string Status { get; set; } = "active";

        /// <summary>
        /// Imagen de perfil del usuario.
        /// </summary>
        public string ProfileImage { get; set; } = "default.png";

        /// <summary>
        /// Idioma preferido del usuario.
        /// </summary>
        public string Language { get; set; } = "en";

        /// <summary>
        /// Número de intentos de inicio de sesión fallidos.
        /// </summary>
        public int FailedLoginAttempts { get; set; } = 0;

        /// <summary>
        /// Fecha del último intento de inicio de sesión fallido.
        /// </summary>
        public DateTime? LastFailedLogin { get; set; }

        /// <summary>
        /// Equipos asociados al usuario.
        /// </summary>
        public ICollection<Team>? Teams { get; set; }

        }
}