namespace NFLFantasy.Api.Models
{
    /// <summary>
    /// Representa una regla de puntuación posible en el sistema.
    /// </summary>
    public class Scoring
    {
    public int ScoringId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public double Points { get; set; }
    public string? Unit { get; set; }
    public string? Description { get; set; }
    }
}
