using NFLFantasy.Api.Data;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using NFLFantasy.Api;

namespace NFLFantasy.Api.Services
{
    public class UserService
    {
        private readonly FantasyContext _context;
        public UserService(FantasyContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="profileImageFileName"></param>
        /// <returns></returns>

        public async Task<(bool Success, string? Error, User? User)> RegisterAsync(RegisterUserDto dto, string? profileImageFileName = null)
        {
            // Validación de email único
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return (false, AppConstants.ErrorEmailAlreadyRegistered, null);

            if (await _context.Users.AnyAsync(u => u.Alias == dto.Alias))
                return (false, AppConstants.ErrorAliasInUse, null);

            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Alias) || string.IsNullOrWhiteSpace(dto.Password))
                return (false, AppConstants.ErrorMissingUserFields, null);

            var passwordHash = HashPassword(dto.Password);

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Alias = dto.Alias,
                PasswordHash = passwordHash,
                ProfileImage = profileImageFileName ?? AppConstants.DefaultProfileImage
                // CreatedAt, Role, Status, Language usan valores por defecto
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return (true, null, user);
        }

        /// <summary>
        /// Inicia sesión de un usuario registrado.
        /// </summary>
        public async Task<(bool Success, string? Error, User? User)> LoginAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return (false, AppConstants.ErrorInvalidCredentials, null);

            if (user.Status == "locked")
                return (false, AppConstants.ErrorAccountLocked, null);

            // Inicializar campos si no existen
            user.FailedLoginAttempts = user.FailedLoginAttempts;
            user.LastFailedLogin = user.LastFailedLogin;

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                user.FailedLoginAttempts++;
                user.LastFailedLogin = DateTime.UtcNow;
                if (user.FailedLoginAttempts >= 5)
                {
                    user.Status = "locked";
                }
                await _context.SaveChangesAsync();
                return (false, AppConstants.ErrorInvalidCredentials, null);
            }

            // Login exitoso
            user.FailedLoginAttempts = 0;
            await _context.SaveChangesAsync();
            return (true, null, user);
        }

        /// <summary>
        /// Hash de la contraseña.
        /// </summary>
        private string HashPassword(string password)
        {
            // BCrypt hash seguro
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        
    }
}
