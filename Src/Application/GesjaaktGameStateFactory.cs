using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces;
using Domain.Interfaces.Games.Gesjaakt;

namespace Application;

public class GesjaaktGameStateFactory : IGameStateFactory
{
    private readonly ILogger<GesjaaktGameState> _logger;

    public GesjaaktGameStateFactory(ILogger<GesjaaktGameState> logger)
    {
        _logger = logger;
    }

    public IGesjaaktGameState Create(IEnumerable<IGesjaaktPlayer> players)
    {
        return new GesjaaktGameState(players, _logger);
    }
}
