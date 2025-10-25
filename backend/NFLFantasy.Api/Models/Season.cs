using System;
using System.Collections.Generic;

namespace NFLFantasy.Api.Models
{
    public class Season
    {
        public int SeasonId { get; set; }
        public string Name { get; set; } = string.Empty; // 1-100 chars
        public int WeeksCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCurrent { get; set; }  //estado actual
        public DateTime CreatedAt { get; set; }
        public ICollection<Week> Weeks { get; set; } = new List<Week>();
    }

    public class Week
    {
        public int WeekId { get; set; }
        public int SeasonId { get; set; }
        public Season Season { get; set; } = null!; //puntero no nulo
        public int Number { get; set; } // 1-based week number
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
