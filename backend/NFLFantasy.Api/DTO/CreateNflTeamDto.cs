using System.ComponentModel.DataAnnotations;

namespace NFLFantasy.Api.DTO
{
    /// <summary>
    /// DTO para la creación manual de equipos NFL por el administrador.
    /// </summary>
    public class CreateNflTeamDto
    {
        /// <summary>
        /// Nombre oficial del equipo NFL.
        /// </summary>
        [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El campo Nombre debe tener máximo 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Ciudad base del equipo NFL.
        /// </summary>
        [Required(ErrorMessage = "El campo Ciudad es obligatorio.")]
        [StringLength(100, ErrorMessage = "El campo Ciudad debe tener máximo 100 caracteres.")]
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Imagen principal del equipo (archivo).
        /// </summary>
        [Required(ErrorMessage = "Debes adjuntar una imagen para el equipo.")]
        public IFormFile Image { get; set; } = null!;
    }
}
