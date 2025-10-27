using System;
using System.Collections.Generic;

namespace NFLFantasy.Api.DTO
{
    public class SearchSeasonDto
    {
        public int SeasonId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int WeeksCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public DateTime CreatedAt { get; set; }

        // Agregamos las semanas, pero sin la referencia circular
        public List<WeekDto> Weeks { get; set; } = new();
    }

    public class WeekDto
    {
        public int WeekId { get; set; }
        public int Number { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
