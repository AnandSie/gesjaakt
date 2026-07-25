using Application.Interfaces;
using Domain.Entities.Game.Qwixx;
using Domain.Interfaces.Games.BaseGame;
using Domain.Interfaces.Games.Qwixx;

namespace Application.Qwixx;

public class QwixxGame : GameOption, IGame<IQwixxPlayer>
{
    private readonly IQwixxGameEventCollector gameEventCollector;
    private QwixxGameDealer _gameDealer;

    public QwixxGame(IQwixxGameEventCollector gameEventCollector)
        : base(typeof(QwixxGame), QwixxRules.MinNumberOfPlayers, QwixxRules.MaxNumberOfPlayers)
    {
        this.gameEventCollector = gameEventCollector;
    }

    public static string Name { get; } = "Qwixx";

    public void PlayWith(IEnumerable<IQwixxPlayer> players)
    {
        var gameState = new QwixxGameState();
        _gameDealer = new QwixxGameDealer(gameState, new QwixxDiceRoller());
        _gameDealer.Add(players);

        // Nothing is attached to gameState: unlike GesjaaktGameState/TakeFiveGameState it raises
        // no events of its own (see IQwixxGameEventCollector).
        gameEventCollector
            .Attach(_gameDealer)
            .Attach(players);

        _gameDealer.Prepare();
        _gameDealer.Play();
    }

    public IOrderedEnumerable<IQwixxPlayer> Results()
    {
        return _gameDealer.GetPlayerResults();
    }
}
