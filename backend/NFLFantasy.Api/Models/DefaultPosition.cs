namespace NFLFantasy.Api.Models
{
    /// <summary>
    /// Asocia una posición por defecto a una liga y la cantidad requerida.
    /// </summary>
    public class DefaultPosition
    {
        public int DefaultPositionId { get; set; }
        public int LeagueId { get; set; }
        public int PositionId { get; set; }
        public int Quantity { get; set; }

        public League? League { get; set; }
        public Position? Position { get; set; }
    }
}
