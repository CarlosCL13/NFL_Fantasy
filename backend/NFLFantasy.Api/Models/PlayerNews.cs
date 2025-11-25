using System;

namespace NFLFantasy.Api.Models
{
    public class PlayerNews
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string Texto { get; set; } = string.Empty;
        public bool IsLesion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string? Resumen { get; set; }
        public int? DesignacionId { get; set; }
        public Designacion? Designacion { get; set; }
        public string Autor { get; set; } = string.Empty;
        public string HoraCreacion { get; set; } = string.Empty;
        public string Cambios { get; set; } = string.Empty;
    }
}
