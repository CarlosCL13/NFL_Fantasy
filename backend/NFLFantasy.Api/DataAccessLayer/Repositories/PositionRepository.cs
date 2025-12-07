using NFLFantasy.Api.Data;
using NFLFantasy.Api.Models;
using System.Collections.Generic;
using System.Linq;

namespace NFLFantasy.Api.DataAccessLayer.Repositories
{
    public interface IPositionRepository
    {
        List<Position> GetAll();
    }

    public class PositionRepository : IPositionRepository
    {
        private readonly FantasyContext _context;
        public PositionRepository(FantasyContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todas las posiciones disponibles.
        /// </summary>
        public List<Position> GetAll()
        {
            return _context.Positions.ToList();
        }
    }
}
