using System.ComponentModel.DataAnnotations;

namespace NFLFantasy.Api.DTO
{   
    /// <summary>
    /// DTO para la creación de una nueva liga de fantasía NFL.
    /// </summary>
    public class CreateLeagueDto

    {

        /// <summary>
        /// Alias único del equipo del comisionado.
        /// </summary>
        [Required, StringLength(30)]
        public string CommissionerAlias { get; set; } = string.Empty;

        /// <summary>
        /// Nombre de la liga.
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "El nombre de la liga es obligatorio y debe tener entre 1 y 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descripción de la liga.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Cantidad máxima de equipos en la liga.
        /// </summary>
        [Required(ErrorMessage = "La cantidad de equipos es obligatoria.")]
        [Range(4, 20, ErrorMessage = "La cantidad de equipos debe ser uno de los siguientes valores: 4, 6, 8, 10, 12, 14, 16, 18 o 20.")]
        public int MaxTeams { get; set; }

        /// <summary>
        /// Contraseña para unirse a la liga.
        /// </summary>
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(12, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 12 caracteres.")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).{8,12}$", ErrorMessage = "La contraseña debe ser alfanumérica, contener al menos una minúscula y una mayúscula.")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de playoffs (4 o 6 equipos).
        /// </summary>
        [Required(ErrorMessage = "Debes indicar el tipo de playoffs (4 o 6 equipos).")]
        [Range(4, 6, ErrorMessage = "El tipo de playoffs debe ser 4 o 6.")]
        public int PlayoffType { get; set; }

        /// <summary>
        /// Nombre del equipo del comisionado.
        /// </summary>
        [Required(ErrorMessage = "El nombre del equipo del comisionado es obligatorio.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "El nombre del equipo debe tener entre 1 y 100 caracteres.")]
        public string CommissionerTeamName { get; set; } = string.Empty;
    }
}
