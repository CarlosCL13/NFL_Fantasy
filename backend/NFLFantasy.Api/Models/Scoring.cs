namespace NFLFantasy.Api.Models
{
    /// <summary>
    /// Representa una regla de puntuación posible en el sistema.
    /// </summary>
    public class Scoring
    {
        /// <summary>
        /// Identificador único de la regla de puntuación.
        /// </summary>
        public int ScoringId { get; set; }

        /// <summary>
        /// Nombre de la regla de puntuación.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Abreviación de la regla de puntuación.
        /// </summary>
        public string Abbreviation { get; set; } = string.Empty;

        /// <summary>
        /// Puntos otorgados por esta regla de puntuación.
        /// </summary>
        public double Points { get; set; }

        /// <summary>
        /// Unidad de medida para los puntos (por ejemplo, "puntos", "yardas", etc.).
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// Descripción de la regla de puntuación.
        /// </summary>
        public string? Description { get; set; }
    }
}
