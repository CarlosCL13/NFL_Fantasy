using Microsoft.AspNetCore.Mvc;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Services;
using NFLFantasy.Api;

namespace NFLFantasy.Api.Controllers
{
    [ApiController]
    [Route("api/users")]

    /// <summary>
    /// Controlador para la gestión de usuarios.
    /// </summary>
    public class UsersController(UserService userService) : ControllerBase
    {
        /// <summary>
        /// Servicio de usuario.
        /// </summary>
        private readonly UserService _userService = userService;

        /// <summary>
        /// Registra un nuevo usuario en la plataforma.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterUserDto dto)
        {
            // Validación del modelo
            if (!ModelState.IsValid)
            {
                // Extrae los errores de validación del modelo
                var errors = ModelState
                    .Where(kvp => kvp.Value != null && kvp.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value != null ? kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray() : new string[0]
                    );

                // Devuelve un BadRequest con los errores detallados
                return BadRequest(new { error = AppConstants.ErrorInvalidRegisterData, detalles = errors });
            }

            // Llama al servicio para registrar el usuario
            var (success, error, user) = await _userService.RegisterAsync(dto);

            // Verifica si hubo un error en el registro
            if (!success)
                return BadRequest(new { error = error ?? "No se pudo registrar el usuario. Por favor, verifica los datos e inténtalo de nuevo." });

            // Devuelve respuesta exitosa
            return Ok(new { message = "Usuario registrado exitosamente.", userId = user!.UserId });
        }

        /// <summary>
        /// Inicia sesión de usuario registrado.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // Validación del modelo
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Datos de inicio de sesión inválidos. Por favor, revisa los campos requeridos." });

            // Llama al servicio de usuario para iniciar sesión
            var (success, error, user, token) = await _userService.LoginAsync(dto.Email, dto.Password);

            // Verifica si hubo un error en el inicio de sesión
            if (!success)
                return BadRequest(new { error = error ?? "No se pudo iniciar sesión. Por favor, verifica tus credenciales e inténtalo de nuevo." });

            // Devuelve respuesta exitosa con el token JWT y el rol
            return Ok(new
            {
                message = "Inicio de sesión exitoso.",
                user = new {
                    userId = user!.UserId,
                    name = user.Name,
                    email = user.Email,
                    alias = user.Alias,
                    profileImage = user.ProfileImage,
                    role = user.Role != null ? user.Role.Name : null
                },
                token
            });
        }
    }
}
