using Domain.Entities.Game.Qwixx;
using Domain.Interfaces.Games.BaseGame;
using Domain.Interfaces.Games.Qwixx;

namespace Application.Qwixx;

// No event collector is attached here, unlike GesjaaktGame/TakeFiveGame - QwixxGameDealer
// deliberately raises no console/UI events yet (see docs/qwixx/status-report.md open items).
public class QwixxGame : GameOption, IGame<IQwixxPlayer>
{
    private QwixxGameDealer _gameDealer;

    public QwixxGame() : base(typeof(QwixxGame), QwixxRules.MinNumberOfPlayers, QwixxRules.MaxNumberOfPlayers)
    {
    }

    public static string Name { get; } = "Qwixx";

    public void PlayWith(IEnumerable<IQwixxPlayer> players)
    {
        var gameState = new QwixxGameState();
        _gameDealer = new QwixxGameDealer(gameState, new QwixxDiceRoller());
        _gameDealer.Add(players);

        _gameDealer.Prepare();
        _gameDealer.Play();
    }

    public IOrderedEnumerable<IQwixxPlayer> Results()
    {
        return _gameDealer.GetPlayerResults();
    }
}
