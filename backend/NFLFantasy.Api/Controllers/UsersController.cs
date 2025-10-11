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
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, error, user) = await _userService.RegisterAsync(dto);
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
