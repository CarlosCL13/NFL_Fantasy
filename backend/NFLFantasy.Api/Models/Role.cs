using System.Collections.Generic;

namespace NFLFantasy.Api.Models
{
    /// <summary>
    /// Rol de usuario (Admin, Usuario, etc.)
    /// </summary>
    public class Role
    {
        /// <summary>
        /// Identificador único del rol.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Nombre del rol.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descripción del rol.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Usuarios asociados a este rol.
        /// </summary>
        public ICollection<User>? Users { get; set; }
    }
}