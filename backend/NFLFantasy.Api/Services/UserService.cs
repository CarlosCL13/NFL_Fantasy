using NFLFantasy.Api.Data;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace NFLFantasy.Api.Services
{
    public class UserService
    {
        private readonly FantasyContext _context;
        public UserService(FantasyContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string? Error, User? User)> RegisterAsync(RegisterUserDto dto, string? profileImageFileName = null)
        {
            // Validación de email único
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return (false, "El correo ya está registrado.", null);

            // Validación de alias único (opcional, quitar si no es necesario)
            if (await _context.Users.AnyAsync(u => u.Alias == dto.Alias))
                return (false, "El alias ya está en uso.", null);

            // Encriptar contraseña
            var passwordHash = HashPassword(dto.Password);

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Alias = dto.Alias,
                PasswordHash = passwordHash,
                ProfileImage = profileImageFileName ?? "default.png"
                // CreatedAt, Role, Status, Language usan valores por defecto
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return (true, null, user);
        }

                public async Task<(bool Success, string? Error, User? User)> LoginAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return (false, "Credenciales inválidas.", null);

            if (user.Status == "locked")
                return (false, "La cuenta está bloqueada por intentos fallidos. Contacte al administrador.", null);

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
                return (false, "Credenciales inválidas.", null);
            }

            // Login exitoso
            user.FailedLoginAttempts = 0;
            await _context.SaveChangesAsync();
            return (true, null, user);
        }

        private string HashPassword(string password)
        {
            // BCrypt hash seguro
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        
    }
}
