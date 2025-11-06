namespace NFLFantasy.Api.Models
{
    /// <summary>
    /// Asocia una posición por defecto a una liga y la cantidad requerida.
    /// </summary>
    public class DefaultPosition
    {

        /// <summary>
        /// Identificador único de la posición por defecto.
        /// </summary>
        public int DefaultPositionId { get; set; }

        /// <summary> 
        /// Identificador de la liga a la que pertenece esta posición por defecto.
        /// </summary>  
        public int LeagueId { get; set; }

        /// <summary>
        /// Identificador de la posición asociada.
        /// </summary>
        public int PositionId { get; set; }

        /// <summary>
        /// Cantidad requerida de esta posición en la liga.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Navegación a la liga.
        /// </summary>
        public League? League { get; set; }

        /// <summary>
        /// Navegación a la posición.
        /// </summary>
        public Position? Position { get; set; }
    }
}
