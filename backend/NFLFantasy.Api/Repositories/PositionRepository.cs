using NFLFantasy.Api.Data;
using NFLFantasy.Api.Models;
using System.Collections.Generic;
using System.Linq;

namespace NFLFantasy.Api.Repositories
{
    public class PositionRepository
    {
        private readonly FantasyContext _context;
        public PositionRepository(FantasyContext context)
        {
            _context = context;
        }

        public List<Position> GetAll()
        {
            return _context.Positions.ToList();
        }
    }
}
