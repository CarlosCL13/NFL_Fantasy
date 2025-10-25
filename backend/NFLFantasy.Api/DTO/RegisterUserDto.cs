using System.ComponentModel.DataAnnotations;

namespace NFLFantasy.Api.DTO
{
    /// <summary>
    /// DTO para el registro de un nuevo usuario.
    /// </summary>
    public class RegisterUserDto
    {

        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre debe tener máximo 50 caracteres.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Correo electrónico del usuario.
        /// </summary>
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [StringLength(50, ErrorMessage = "El correo electrónico debe tener máximo 50 caracteres.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Alias del usuario.
        /// </summary>
        [Required(ErrorMessage = "El alias es obligatorio.")]
        [StringLength(50, ErrorMessage = "El alias debe tener máximo 50 caracteres.")]
        public string Alias { get; set; } = string.Empty;

        /// <summary>
        /// Contraseña del usuario.
        /// </summary>
        [Required, StringLength(12, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 12 caracteres."), RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).{8,12}$", ErrorMessage = "La contraseña debe ser alfanumérica, contener al menos una minúscula y una mayúscula.")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Confirmación de la contraseña del usuario.
        /// </summary>
        [Required]
        [Compare("Password", ErrorMessage = "La confirmación de contraseña no coincide.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// Imagen de perfil del usuario (opcional).
        /// </summary>
        public IFormFile? ProfileImage { get; set; }
    }
}