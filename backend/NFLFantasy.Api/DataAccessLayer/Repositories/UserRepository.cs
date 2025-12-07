using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NFLFantasy.Api.Data;
using NFLFantasy.Api.Models;

namespace NFLFantasy.Api.DataAccessLayer.Repositories
{
    public interface IUserRepository
    {
        Task<bool> EmailExistsAsync(string email);
        Task<bool> AliasExistsAsync(string alias);
        Task<Role?> GetManagerRoleAsync();
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task<User?> GetUserByEmailAsync(string email);
    }

    public class UserRepository : IUserRepository
    {
        private readonly FantasyContext _context;
        public UserRepository(FantasyContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Verifica si ya existe un usuario con el correo dado.
        /// </summary>
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        /// <summary>
        /// Verifica si ya existe un usuario con el alias dado.
        /// </summary>
        public async Task<bool> AliasExistsAsync(string alias)
        {
            return await _context.Users.AnyAsync(u => u.Alias == alias);
        }

        /// <summary>
        /// Obtiene el rol de 'manager'y lo devuelve.
        /// </summary>
        public async Task<Role?> GetManagerRoleAsync()
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Name == "manager");
        }

        /// <summary>
        /// Agrega un nuevo usuario a la base de datos.
        /// </summary>
        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Actualiza un usuario existente en la base de datos.
        /// </summary>
        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtiene un usuario por su correo electrónico.
        /// </summary>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}