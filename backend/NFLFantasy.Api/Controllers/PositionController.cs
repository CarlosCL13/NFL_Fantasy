using Microsoft.AspNetCore.Mvc;
using NFLFantasy.Api.DataAccessLayer.Repositories;

namespace NFLFantasy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PositionController : ControllerBase
    {
        private readonly PositionRepository _repository;
        public PositionController(PositionRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var positions = _repository.GetAll();
            return Ok(positions);
        }
    }
}
