namespace NFLFantasy.Api.Models
{
    /// <summary>
    /// Asocia una regla de puntuación por defecto a una liga y el valor asignado.
    /// </summary>
    public class DefaultScoring
    {
        public int DefaultScoringId { get; set; }
        public int LeagueId { get; set; }
        public int ScoringId { get; set; }
        public double Value { get; set; }

        public League? League { get; set; }
        public Scoring? Scoring { get; set; }
    }
}
