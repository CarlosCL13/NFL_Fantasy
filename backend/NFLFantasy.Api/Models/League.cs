using System;
using System.Collections.Generic;

namespace NFLFantasy.Api.Models
{
    /// <summary>
    /// Representa una liga de fantasía NFL.
    /// </summary>
    public class League
    {
        /// <summary>
        /// Indica si la liga está activa.
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Identificador único de la liga.
        /// </summary>
        public int LeagueId { get; set; }

        /// <summary>
        /// Nombre de la liga.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descripción de la liga.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Número máximo de equipos permitidos en la liga.
        /// </summary>
        public int MaxTeams { get; set; }

        /// <summary>
        /// Contraseña para unirse a la liga.
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de creación de la liga (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Estado actual de la liga.
        /// </summary>
        public string Status { get; set; } = "Pre-Draft";

        /// <summary>
        /// Relación con Season.
        /// </summary>
        public int SeasonId { get; set; }

        /// <summary>
        /// Temporada a la que pertenece la liga.
        /// </summary>
        public Season Season { get; set; } = null!;

        /// <summary>
        /// Relación con User (Comisionado).
        /// </summary>
        public int CommissionerId { get; set; }

        /// <summary>
        /// Usuario que actúa como comisionado de la liga.
        /// </summary>
        public User Commissioner { get; set; } = null!;

        /// <summary>
        /// Configuraciones específicas de la liga.
        /// </summary>
        public int PlayoffType { get; set; } // 4 o 6

        /// <summary>
        /// Permitir puntos decimales en las puntuaciones.
        /// </summary>
        public bool AllowDecimalPoints { get; set; } = true;

        /// <summary>
        /// Posiciones predeterminadas para los equipos.
        /// </summary>
        public string DefaultPositions { get; set; } = string.Empty;

        /// <summary>
        /// Sistema de puntuación predeterminado para la liga.
        /// </summary>
        public string DefaultScoring { get; set; } = string.Empty;

        /// <summary>
        /// Indica si la fecha límite de intercambios está activa.
        /// </summary>
        public bool TradeDeadlineActive { get; set; } = false;

        /// <summary>
        /// Número máximo de intercambios permitidos por equipo.
        /// </summary>
        public int? MaxTradesPerTeam { get; set; } = null;

        /// <summary>
        /// Número máximo de agentes libres permitidos por equipo.
        /// </summary>
        public int? MaxFreeAgentsPerTeam { get; set; } = null;

        /// <summary>
        /// Colección de equipos que pertenecen a la liga.
        /// </summary>
        public ICollection<Team> Teams { get; set; } = new List<Team>();
    }
}
