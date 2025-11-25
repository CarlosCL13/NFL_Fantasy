using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.Models;
using NFLFantasy.Api.Validators;
using NFLFantasy.Api.Data;
using NFLFantasy.Api.DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;

namespace NFLFantasy.Api.Services
{
    public class PlayerNewsService
    {
        private readonly FantasyContext _context;
        private readonly IPlayerNewsRepository _newsRepository;
        /// <summary>
        /// Constructor del servicio PlayerNewsService.
        /// </summary>
        public PlayerNewsService(FantasyContext context, IPlayerNewsRepository newsRepository)
        {
            _context = context;
            _newsRepository = newsRepository;
        }

        /// <summary>
        /// Agrega una noticia para un jugador específico.
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="autor"></param>
        /// <returns></returns>
        public async Task<(bool Success, List<string> Errors)> AddNewsAsync(CreatePlayerNewsDto dto, string autor)
        {
            var errors = PlayerNewsValidator.Validate(dto);
            if (errors.Any())
                return (false, errors);

            var player = await _context.NflPlayers.FirstOrDefaultAsync(p => p.NflPlayerId == dto.PlayerId);
            if (player == null)
                return (false, new List<string> { "El jugador no existe." });
            if (!player.IsActive)
                return (false, new List<string> { "El jugador está inactivo." });

            var news = new PlayerNews
            {
                PlayerId = dto.PlayerId,
                Texto = dto.Texto,
                IsLesion = dto.IsLesion,
                FechaCreacion = DateTime.UtcNow,
                Resumen = dto.IsLesion ? dto.Resumen : null,
                Designacion = dto.IsLesion ? dto.Designacion : null,
                Autor = autor,
                Auditoria = $"Creado por {autor} el {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}"
            };

            // La designación solo se guarda en PlayerNews, no en NflPlayer.

            await _newsRepository.AddAsync(news);
            await _context.SaveChangesAsync();
            return (true, new List<string>());
        }
        
        /// <summary>
        /// Obtiene las noticias de un jugador específico.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public async Task<List<PlayerNews>> GetNewsByPlayerAsync(int playerId)
        {
            return await _newsRepository.GetByPlayerAsync(playerId);
        }
    }
}
