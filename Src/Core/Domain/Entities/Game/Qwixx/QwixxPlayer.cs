using Domain.Interfaces.Games.Qwixx;

namespace Domain.Entities.Game.Qwixx;

// See docs/qwixx/rules.md for the rule IDs (QX-###) referenced from QwixxPlayerTests.
// Turn-level concerns (QX-012..QX-014: one mark per row per turn, who takes a penalty) are
// deliberately out of scope here — they belong to whatever orchestrates a turn (QwixxGameDealer),
// not to the player's own score sheet.
public class QwixxPlayer : IQwixxPlayer
{
    private readonly IQwixxThinker _thinker;
    private readonly Dictionary<QwixxColor, QwixxRow> _rows;
    private int _penalties;

    public QwixxPlayer(IQwixxThinker thinker)
    {
        _thinker = thinker;
        _rows = Enum.GetValues<QwixxColor>().ToDictionary(color => color, color => new QwixxRow(color));
    }

    public string Name => _thinker.Name;

    // QX-002: one row per color. Must return the same instance on every call for a given color,
    // so marks made through one call are visible through the next.
    public QwixxRow Row(QwixxColor color) => _rows[color];

    // QX-018
    public int Penalties => _penalties;

    public void AddPenalty()
    {
        _penalties++;
    }

    // QX-020
    public bool HasMaxPenalties => _penalties >= QwixxRules.MaxPenalties;

    // QX-030/QX-031: sum of all 4 row scores minus the penalty deduction.
    public int Score => _rows.Values.Sum(row => row.Score) - _penalties * QwixxRules.PenaltyPoints;

    // QX-009: delegates to the injected thinker. Called for every player, every turn.
    public QwixxColor? DecideWhiteMark(IQwixxReadOnlyGameState gameState, int whiteSum)
    {
        return _thinker.DecideWhiteMark(gameState, whiteSum);
    }

    // QX-010: delegates to the injected thinker. Only called for the active (rolling) player.
    public QwixxMark? DecideColoredMark(IQwixxReadOnlyGameState gameState, QwixxDiceRoll roll)
    {
        return _thinker.DecideColoredMark(gameState, roll);
    }

    // QX-022/QX-023: delegates to the injected thinker. Only called when the chosen mark
    // makes locking possible.
    public bool DecideToLock(IQwixxReadOnlyGameState gameState, QwixxColor color)
    {
        return _thinker.DecideToLock(gameState, color);
    }

    public IQwixxReadOnlyPlayer AsReadOnly()
    {
        return new QwixxReadOnlyPlayer(this);
    }
}
