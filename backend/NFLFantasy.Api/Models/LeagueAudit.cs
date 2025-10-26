using System;

namespace NFLFantasy.Api.Models
{
    /// </summary>
    /// Registro de auditoría para acciones sobre una liga
    /// </summary>
    public class LeagueAudit
    {
        // Id único del registro de auditoría
        public int LeagueAuditId { get; set; }

        // Id del usuario que realizó la acción
        public int UserId { get; set; }

        // Id de la liga sobre la que se realizó la acción
        public int LeagueId { get; set; }

        // Acción realizada (ejemplo: "Join")
        public string Action { get; set; } = string.Empty;

        // Fecha y hora de la acción (UTC)
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
