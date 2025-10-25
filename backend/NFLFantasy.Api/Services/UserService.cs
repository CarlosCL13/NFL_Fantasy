using NFLFantasy.Api.Data;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using NFLFantasy.Api;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace NFLFantasy.Api.Services
{
    /// <summary>
    /// Servicio para la gestión de usuarios (registro, login, etc.)
    /// </summary>
    public class UserService
    {
        //Referencia al contexto de la base de datos
        private readonly FantasyContext _context;

        //Referencia a la configuración de la aplicación
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor del servicio UserService.
        /// </summary>
        public UserService(FantasyContext context, IConfiguration configuration)
        {
            _context = context; //Inicializa el contexto de la base de datos
            _configuration = configuration; //Inicializa la configuración de la aplicación
        }

        /// <summary>
        /// Registra un nuevo usuario.
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="profileImageFileName"></param>
        /// <returns></returns>
        /// <remarks>
        /// Este método registra un nuevo usuario en el sistema.
        /// </remarks>
        public async Task<(bool Success, string? Error, User? User)> RegisterAsync(RegisterUserDto dto, string? profileImageFileName = null)
        {
            // Validación de email único
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return (false, AppConstants.ErrorEmailAlreadyRegistered, null);

            // Validación de alias único
            if (await _context.Users.AnyAsync(u => u.Alias == dto.Alias))
                return (false, AppConstants.ErrorAliasInUse, null);

            // Validación de campos obligatorios
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Alias) || string.IsNullOrWhiteSpace(dto.Password))
                return (false, AppConstants.ErrorMissingUserFields, null);

            // Hash de la contraseña
            var passwordHash = HashPassword(dto.Password);

            // Crear usuario 
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Alias = dto.Alias,
                PasswordHash = passwordHash,
                ProfileImage = profileImageFileName ?? AppConstants.DefaultProfileImage
                // CreatedAt, Role, Status, Language usan valores por defecto
            };

            // Guardar en la base de datos
            _context.Users.Add(user);           //Agrega el nuevo usuario al contexto
            await _context.SaveChangesAsync();  //Guarda los cambios en la base de datos
            return (true, null, user);          //Devuelve éxito y el usuario creado
        }

        /// <summary>
        /// Inicia sesión de un usuario.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <remarks>
        /// Este método autentica a un usuario y genera un token JWT.
        /// </remarks>
        public async Task<(bool Success, string? Error, User? User, string? Token)> LoginAsync(string email, string password)
        {
            // Buscar usuario por email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            // Validar usuario encontrado
            if (user == null)
                return (false, AppConstants.ErrorInvalidCredentials, null, null);

            // Verificar si la cuenta está bloqueada
            if (user.Status == "locked")
                return (false, AppConstants.ErrorAccountLocked, null, null);

            // Inicializar campos si no existen
            user.FailedLoginAttempts = user.FailedLoginAttempts;
            user.LastFailedLogin = user.LastFailedLogin;

            // Verificar contraseña
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                user.FailedLoginAttempts++;                 // Incrementar intentos fallidos
                user.LastFailedLogin = DateTime.UtcNow;     // Actualizar fecha del último intento fallido

                // Verificar si se debe bloquear la cuenta
                if (user.FailedLoginAttempts >= 5)
                {
                    user.Status = "locked";
                }

                // Guardar cambios
                await _context.SaveChangesAsync();
                return (false, AppConstants.ErrorInvalidCredentials, null, null);
            }

            // Login exitoso
            user.FailedLoginAttempts = 0;
            await _context.SaveChangesAsync();

            // Generar token JWT
            var token = GenerateJwtToken(user);
            return (true, null, user, token);
        }


        /// <summary>
        /// Genera un token JWT para el usuario autenticado.
        /// </summary>
        private string GenerateJwtToken(User user)
        {
            // Obtiene la clave secreta para firmar el JWT desde la configuración (appsettings.json)
            var key = _configuration["Jwt:Key"];

            // Validar clave
            if (string.IsNullOrWhiteSpace(key) || key.Length < 32)
                throw new Exception("La clave JWT no está configurada correctamente o es demasiado corta. Verifica appsettings.json (Jwt:Key) y asegúrate de que tenga al menos 32 caracteres.");

            // Convertir clave a bytes
            var keyBytes = Encoding.ASCII.GetBytes(key);

            // Crear los claims para el token JWT
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Crear el token JWT
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(12),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            // Crear el manejador de tokens
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Genera un hash seguro de la contraseña usando BCrypt.
        /// </summary>S
        private string HashPassword(string password)
        {
            // BCrypt hash seguro
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        
    }
}
