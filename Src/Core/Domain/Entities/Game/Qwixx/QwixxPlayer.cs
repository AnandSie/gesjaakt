using Domain.Interfaces.Games.Qwixx;

namespace Domain.Entities.Game.Qwixx;

// See docs/qwixx/rules.md for the rule IDs (QX-###) referenced from QwixxPlayerTests.
// Turn-level concerns (QX-012..QX-014: one mark per row per turn, who takes a penalty) are
// deliberately out of scope here — they belong to whatever orchestrates a turn (QwixxGameDealer),
// not to the player's own score sheet.
public class QwixxPlayer : IQwixxPlayer
{
    public QwixxPlayer(IQwixxThinker thinker)
    {
        throw new NotImplementedException();
    }

    public string Name => throw new NotImplementedException();

    // QX-002: one row per color. Must return the same instance on every call for a given color,
    // so marks made through one call are visible through the next.
    public QwixxRow Row(QwixxColor color)
    {
        throw new NotImplementedException();
    }

    // QX-018
    public int Penalties => throw new NotImplementedException();

    public void AddPenalty()
    {
        throw new NotImplementedException();
    }

    // QX-020
    public bool HasMaxPenalties => throw new NotImplementedException();

    // QX-030/QX-031: sum of all 4 row scores minus the penalty deduction.
    public int Score => throw new NotImplementedException();

    // QX-009: delegates to the injected thinker. Called for every player, every turn.
    public QwixxColor? DecideWhiteMark(IQwixxReadOnlyGameState gameState, int whiteSum)
    {
        throw new NotImplementedException();
    }

    // QX-010: delegates to the injected thinker. Only called for the active (rolling) player.
    public QwixxMark? DecideColoredMark(IQwixxReadOnlyGameState gameState, QwixxDiceRoll roll)
    {
        throw new NotImplementedException();
    }

    // QX-022/QX-023: delegates to the injected thinker. Only called when the chosen mark
    // makes locking possible.
    public bool DecideToLock(IQwixxReadOnlyGameState gameState, QwixxColor color)
    {
        throw new NotImplementedException();
    }

    public IQwixxReadOnlyPlayer AsReadOnly()
    {
        throw new NotImplementedException();
    }
}
