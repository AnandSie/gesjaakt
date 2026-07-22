using Domain.Interfaces.Games.Qwixx;

namespace Domain.Entities.Game.Qwixx;

// See docs/qwixx/rules.md for the rule IDs (QX-###) referenced from QwixxGameStateTests.
// Dice rolling itself is deliberately not modeled here: QwixxDiceRoll is created and passed
// directly into Player.DecideWhiteMark/DecideColoredMark by whatever orchestrates a turn
// (QwixxGameDealer), so this class stays dice-agnostic.
public class QwixxGameState : IQwixxGameState
{
    public QwixxGameState()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IQwixxPlayer> Players => throw new NotImplementedException();

    public void AddPlayer(IQwixxPlayer newPlayer)
    {
        throw new NotImplementedException();
    }

    // QX-007: the active roller; passes to the left via NextPlayer().
    public IQwixxPlayer PlayerOnTurn => throw new NotImplementedException();

    public void NextPlayer()
    {
        throw new NotImplementedException();
    }

    // QX-024/QX-025: once any player locks a row, that color is locked for everyone —
    // called by whoever orchestrates a turn when a player's Row(color).Lock() succeeds.
    public void LockColor(QwixxColor color)
    {
        throw new NotImplementedException();
    }

    public bool IsColorLocked(QwixxColor color)
    {
        throw new NotImplementedException();
    }

    // QX-026: 2+ locked colors, or any player at the penalty limit.
    public bool IsGameOver => throw new NotImplementedException();

    public IQwixxReadOnlyGameState AsReadOnly()
    {
        throw new NotImplementedException();
    }
}
