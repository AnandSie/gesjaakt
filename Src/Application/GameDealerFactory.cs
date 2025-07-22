using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces;
using Domain.Interfaces.Games.Gesjaakt;

namespace Application;

public class GameDealerFactory : IGameDealerFactory
{
    private readonly ILogger<GesjaaktGameDealer> _logger;
    private readonly Func<IGesjaaktGameState> _gameStateFactory;

    public GameDealerFactory(ILogger<GesjaaktGameDealer> logger, Func<IGesjaaktGameState> gameStateFactory)
    {
        _logger = logger;
        _gameStateFactory = gameStateFactory;
    }

    public IGameDealer<IGesjaaktPlayerState> Create(IEnumerable<IGesjaaktPlayer> players)
    {
        return new GesjaaktGameDealer(players, _gameStateFactory(), _logger);
    }
}
