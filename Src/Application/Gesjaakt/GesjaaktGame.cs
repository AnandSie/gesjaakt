using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces;
using Domain.Interfaces.Games.BaseGame;
using Domain.Interfaces.Games.Gesjaakt;

namespace Application.Gesjaakt;

public class GesjaaktGame : IGame<IGesjaaktPlayer>
{
    private readonly GesjaaktGameEventOrchestrator _gameEventOrchestrator;
    private GesjaaktGameDealer _gameDealer;

    public GesjaaktGame(GesjaaktGameEventOrchestrator gameEventOrchestrator)
    {
        _gameEventOrchestrator = gameEventOrchestrator;
    }

    // TODO: consider returning the results straightway
    public void PlayWith(IEnumerable<IGesjaaktPlayer> players)
    {
        var gameState = new GesjaaktGameState();
        _gameDealer = new GesjaaktGameDealer(gameState);
        _gameDealer.Add(players);
        
        _gameEventOrchestrator.Attach(gameState, _gameDealer);
        
        _gameDealer.Prepare();
        _gameDealer.Play();
    }

    public IOrderedEnumerable<IGesjaaktPlayer> Results()
    {
        return _gameDealer.GetPlayerResults();
    }
}
