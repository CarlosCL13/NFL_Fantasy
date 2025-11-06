using System.Collections.Generic;

namespace NFLFantasy.Api.Models
{
    /// <summary>
    /// Rol de usuario (Admin, Usuario, etc.)
    /// </summary>
    public class Role
    {
        public int RoleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Relación: Un rol puede estar asignado a varios usuarios
        public ICollection<User>? Users { get; set; }
    }
}