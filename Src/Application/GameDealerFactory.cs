using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces;

namespace Application;

public class GameDealerFactory : IGameDealerFactory
{
    private readonly ILogger<GesjaaktGameDealer> _logger;
    private readonly Func<IGameState> _gameStateFactory;

    public GameDealerFactory(ILogger<GesjaaktGameDealer> logger, Func<IGameState> gameStateFactory)
    {
        _logger = logger;
        _gameStateFactory = gameStateFactory;
    }

    public IGameDealer Create(IEnumerable<IPlayer> players)
    {
        return new GesjaaktGameDealer(players, _gameStateFactory(), _logger);
    }
}
