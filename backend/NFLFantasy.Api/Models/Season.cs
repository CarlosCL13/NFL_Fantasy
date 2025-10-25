using System;
using System.Collections.Generic;

namespace NFLFantasy.Api.Models
{
    /// <summary>
    /// Representa una temporada de la NFL en el sistema de fantasía.
    /// </summary>
    public class Season
    {
        /// <summary>
        /// Identificador único de la temporada.
        /// </summary>
        public int SeasonId { get; set; }

        /// <summary>
        /// Nombre de la temporada.
        /// </summary>
        public string Name { get; set; } = string.Empty; // 1-100 chars

        /// <summary>
        /// Cantidad de semanas en la temporada.
        /// </summary>
        public int WeeksCount { get; set; }

        /// <summary>
        /// Fecha de inicio de la temporada (UTC).
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Fecha de finalización de la temporada (UTC).
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Indica si esta es la temporada actual.
        /// </summary>
        public bool IsCurrent { get; set; }  //estado actual

        /// <summary>
        /// Fecha de creación del registro (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Colección de semanas asociadas a esta temporada.
        /// </summary>
        public ICollection<Week> Weeks { get; set; } = new List<Week>();
    }

    /// <summary>
    /// Representa una semana dentro de una temporada de la NFL.
    /// </summary>
    public class Week
    {   
        /// <summary>
        /// Identificador único de la semana.
        /// </summary>
        public int WeekId { get; set; }

        /// <summary>
        /// Relación con Season.
        /// </summary>
        public int SeasonId { get; set; }

        /// <summary>
        /// Relación con Season.
        /// </summary>
        public Season Season { get; set; } = null!; //puntero no nulo

        /// <summary>
        /// Número de la semana dentro de la temporada. (1-based)
        /// </summary>
        public int Number { get; set; } // 1-based week number

        /// <summary>
        /// Fecha de inicio de la semana (UTC).
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Fecha de finalización de la semana (UTC).
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}
