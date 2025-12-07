using Microsoft.AspNetCore.Mvc;
using NFLFantasy.Api.DataAccessLayer.Repositories;

namespace NFLFantasy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PositionController : ControllerBase
    {
        /// <summary>
        /// Repositorio para acceder a las posiciones.
        /// </summary>
        private readonly IPositionRepository _repository;

        /// <summary>
        /// Constructor del controlador de posiciones.
        /// </summary>
        /// <param name="repository"></param>
        public PositionController(IPositionRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Obtiene todas las posiciones disponibles.
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            var positions = _repository.GetAll();
            return Ok(positions);
        }
    }
}
