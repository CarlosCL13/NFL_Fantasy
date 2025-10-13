using Microsoft.AspNetCore.Mvc;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Services;

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
                return BadRequest(ModelState);

            string? profileImageFileName = null;
            if (dto.ProfileImage != null && dto.ProfileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "users");
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
                return BadRequest(new { error });

            return Ok(new { message = "Usuario registrado exitosamente", userId = user!.UserId });
        }

        /// <summary>
        /// Inicia sesión de usuario registrado.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, error, user) = await _userService.LoginAsync(dto.Email, dto.Password);
            if (!success)
                return BadRequest(new { error });

            return Ok(new { message = "Inicio de sesión exitoso", userId = user!.UserId, alias = user.Alias });
        }
    }
}
