namespace NFLFantasy.Api.Models
{
    /// <summary>
    /// Asocia una regla de puntuación por defecto a una liga y el valor asignado.
    /// </summary>
    public class DefaultScoring
    {
        /// <summary>
        /// Identificador único de la regla de puntuación por defecto.
        /// </summary>
        public int DefaultScoringId { get; set; }

        /// <summary>
        /// Identificador de la liga a la que pertenece esta regla de puntuación por defecto.   
        /// </summary>
        public int LeagueId { get; set; }

        /// <summary>
        /// Identificador de la regla de puntuación asociada.
        /// </summary>
        public int ScoringId { get; set; }

        /// <summary>
        /// Valor asignado a esta regla de puntuación en la liga.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Navegación a la liga.
        /// </summary>
        public League? League { get; set; }

        /// <summary>
        /// Navegación a la regla de puntuación.
        /// </summary>
        public Scoring? Scoring { get; set; }
    }
}
