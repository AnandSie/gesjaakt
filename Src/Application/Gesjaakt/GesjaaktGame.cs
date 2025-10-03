using Application.Interfaces;
using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces.Games.BaseGame;
using Domain.Interfaces.Games.Gesjaakt;

namespace Application.Gesjaakt;

public class GesjaaktGame(IGesjaaktGameEventCollector gameEventCollector) : IGame<IGesjaaktPlayer>
{
    private GesjaaktGameDealer _gameDealer;

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
