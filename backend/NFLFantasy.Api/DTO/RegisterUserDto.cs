using System.ComponentModel.DataAnnotations;

namespace NFLFantasy.Api.DTO
{
    public class RegisterUserDto
    {
        [Required, StringLength(50, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(50), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Alias { get; set; } = string.Empty;

    [Required, StringLength(12, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 12 caracteres."), RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).{8,12}$", ErrorMessage = "La contraseña debe ser alfanumérica, contener al menos una minúscula y una mayúscula.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password", ErrorMessage = "La confirmación de contraseña no coincide.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}