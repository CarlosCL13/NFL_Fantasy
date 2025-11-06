namespace NFLFantasy.Api.Models
{
    /// <summary>
    /// Representa una posición posible en el sistema (ej: QB, RB, WR, etc.)
    /// </summary>
    public class Position
    {
        /// <summary>
        /// Identificador único de la posición.
        /// </summary>
        public int PositionId { get; set; }

        /// <summary>
        /// Nombre de la posición.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Abreviación de la posición.
        /// </summary>
        public string Abbreviation { get; set; } = string.Empty;

        /// <summary>
        /// Descripción de la posición.
        /// </summary>
        public string? Description { get; set; }
    }
}
