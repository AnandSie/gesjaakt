using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces.Games.BaseGame;
using Domain.Interfaces.Games.Gesjaakt;

namespace Application.Gesjaakt;

public class GesjaaktGame(GesjaaktGameEventOrchestrator gameEventOrchestrator) : IGame<IGesjaaktPlayer>
{
    private GesjaaktGameDealer _gameDealer;

    public void PlayWith(IEnumerable<IGesjaaktPlayer> players)
    {
        var gameState = new GesjaaktGameState();
        _gameDealer = new GesjaaktGameDealer(gameState);
        _gameDealer.Add(players);

        gameEventOrchestrator
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
