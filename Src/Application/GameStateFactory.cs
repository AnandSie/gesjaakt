using Domain.Entities.Game;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class GameStateFactory : IGameStateFactory
    {
        private readonly ILogger<GameState> _logger;

        public GameStateFactory(ILogger<GameState> logger)
        {
            _logger = logger;
        }

        public IGameState Create(IEnumerable<IPlayer> players)
        {
            return new GameState(players, _logger);
        }
    }
}
