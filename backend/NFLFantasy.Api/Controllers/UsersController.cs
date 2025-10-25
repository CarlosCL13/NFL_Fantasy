using Microsoft.AspNetCore.Mvc;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Services;
using NFLFantasy.Api;

namespace NFLFantasy.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController(UserService userService) : ControllerBase
    {
        private readonly UserService _userService = userService;

        /// <summary>
        /// Registra un nuevo usuario en la plataforma.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kvp => kvp.Value != null && kvp.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value != null ? kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray() : new string[0]
                    );
                return BadRequest(new { error = AppConstants.ErrorInvalidRegisterData, detalles = errors });
            }

            string? profileImageFileName = null;
            if (dto.ProfileImage != null && dto.ProfileImage.Length > 0)
            {
                var extension = Path.GetExtension(dto.ProfileImage.FileName).ToLowerInvariant();
                if (!AppConstants.AllowedImageExtensions.Contains(extension))
                    return BadRequest(new { error = AppConstants.ErrorProfileImageFormat });

                if (dto.ProfileImage.Length > AppConstants.MaxImageFileSize)
                    return BadRequest(new { error = AppConstants.ErrorProfileImageTooLarge });

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), AppConstants.UsersImageFolder.Replace("/", Path.DirectorySeparatorChar.ToString()));
                Directory.CreateDirectory(uploadsFolder);
                profileImageFileName = $"{Guid.NewGuid()}_{dto.ProfileImage.FileName}";
                var filePath = Path.Combine(uploadsFolder, profileImageFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ProfileImage.CopyToAsync(stream);
                }
            }

            var (success, error, user) = await _userService.RegisterAsync(dto, profileImageFileName);
            if (!success)
                return BadRequest(new { error = error ?? "No se pudo registrar el usuario. Por favor, verifica los datos e inténtalo de nuevo." });

            return Ok(new { message = "Usuario registrado exitosamente.", userId = user!.UserId });
        }

        /// <summary>
        /// Inicia sesión de usuario registrado.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Datos de inicio de sesión inválidos. Por favor, revisa los campos requeridos." });

            var (success, error, user) = await _userService.LoginAsync(dto.Email, dto.Password);
            if (!success)
                return BadRequest(new { error = error ?? "No se pudo iniciar sesión. Por favor, verifica tus credenciales e inténtalo de nuevo." });

            return Ok(new { message = "Inicio de sesión exitoso.", userId = user!.UserId, alias = user.Alias });
        }
    }
}
