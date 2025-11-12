using System;

namespace NFLFantasy.Api.Models
{
    /// </summary>
    /// Registro de auditoría para acciones sobre una liga
    /// </summary>
    public class LeagueAudit
    {
        /// <summary>
        /// Identificador único del registro de auditoría.
        /// </summary>
        public int LeagueAuditId { get; set; }

        /// <summary>
        /// Id del usuario que realizó la acción.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Id de la liga sobre la que se realizó la acción.
        /// </summary>
        public int LeagueId { get; set; }

        /// <summary>
        /// Acción realizada (ejemplo: "Join")
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// Fecha y hora de la acción (UTC)
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
