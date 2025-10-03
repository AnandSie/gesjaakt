using Application.Interfaces;
using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces.Games.BaseGame;
using Domain.Interfaces.Games.Gesjaakt;

namespace Application.Gesjaakt;

public class GesjaaktGame : GameOption, IGame<IGesjaaktPlayer>
{
    private readonly IGesjaaktGameEventCollector gameEventCollector;
    private GesjaaktGameDealer _gameDealer;

    // REFACTOR: min/max players from config object
    public GesjaaktGame(IGesjaaktGameEventCollector gameEventCollector) : base(typeof(GesjaaktGame), 3, 7)
    {
        this.gameEventCollector = gameEventCollector;
    }

    public static string Name { get; } = "Gejaakt";

    public void PlayWith(IEnumerable<IGesjaaktPlayer> players)
    {
        var gameState = new GesjaaktGameState();
        _gameDealer = new GesjaaktGameDealer(gameState); // REFACTOR - DI
        _gameDealer.Add(players);

        gameEventCollector
            .Attach(gameState)
            .Attach(_gameDealer);

        _gameDealer.Prepare();
        _gameDealer.Play();
    }

    public IOrderedEnumerable<IGesjaaktPlayer> Results()
    {
        return _gameDealer.GetPlayerResults();
    }
}
