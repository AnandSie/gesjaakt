using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces;

namespace Application;

public class GesjaaktGameStateFactory : IGameStateFactory
{
    private readonly ILogger<GesjaaktGameState> _logger;

    public GesjaaktGameStateFactory(ILogger<GesjaaktGameState> logger)
    {
        _logger = logger;
    }

    public IGameState Create(IEnumerable<IPlayer> players)
    {
        return new GesjaaktGameState(players, _logger);
    }
}
