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
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Ciudad base del equipo NFL.
    /// </summary>
    [Required, StringLength(100)]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Imagen principal del equipo (archivo).
    /// </summary>
    [Required]
    public IFormFile Image { get; set; } = null!;
    }
}
