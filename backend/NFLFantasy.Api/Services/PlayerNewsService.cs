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
            var errors = PlayerNewsValidator.Validate(dto, _context);
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
                HoraCreacion = DateTime.UtcNow.ToString("HH:mm:ss"),
                Cambios = $"Creado por {autor}",
                Resumen = dto.IsLesion ? dto.Resumen : null,
                DesignacionId = dto.IsLesion ? dto.DesignacionId : null,
                Autor = autor
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
            // Incluye la designación en el resultado
            return await _context.PlayerNews
                .Include(n => n.Designacion)
                .Where(n => n.PlayerId == playerId)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene la designación actual de lesión para un jugador.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public async Task<Designacion?> GetCurrentDesignacionAsync(int playerId)
        {
            // Busca la última noticia de lesión con designación para el jugador
            var news = await _context.PlayerNews
                .Where(n => n.PlayerId == playerId && n.IsLesion && n.DesignacionId != null)
                .OrderByDescending(n => n.FechaCreacion)
                .FirstOrDefaultAsync();
            if (news?.DesignacionId == null)
                return null;
            return await _context.Designaciones.FindAsync(news.DesignacionId);
        }
    }
}
