namespace NFLFantasy.Api.Models
{
    /// <summary>
    /// Representa una posición posible en el sistema (ej: QB, RB, WR, etc.)
    /// </summary>
    public class Position
    {
    public int PositionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string? Description { get; set; }
    }
}
