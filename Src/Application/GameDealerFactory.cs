using Domain.Entities.Game;
using Domain.Interfaces;

namespace Application
{
    public class GameDealerFactory : IGameDealerFactory
    {
        private readonly ILogger<GameDealer> _logger;
        private readonly Func<IGameState> _gameStateFactory;

        public GameDealerFactory(ILogger<GameDealer> logger, Func<IGameState> gameStateFactory)
        {
            _logger = logger;
            _gameStateFactory = gameStateFactory;
        }

        public IGameDealer Create(IEnumerable<IPlayer> players)
        {
            return new GameDealer(players, _gameStateFactory, _logger);
        }
    }
}
