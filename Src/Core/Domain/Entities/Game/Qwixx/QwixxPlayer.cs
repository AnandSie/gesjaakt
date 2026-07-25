using Domain.Entities.Events;
using Domain.Interfaces.Games.Qwixx;

namespace Domain.Entities.Game.Qwixx;

// See docs/qwixx/rules.md for the rule IDs (QX-###) referenced from QwixxPlayerTests.
// Turn-level concerns (QX-013/QX-014: who takes a penalty) are deliberately out of scope here —
// they belong to whatever orchestrates a turn (QwixxGameDealer), not to the player's own score sheet.
public class QwixxPlayer : IQwixxPlayer
{
    private readonly IQwixxThinker _thinker;
    private readonly Dictionary<QwixxColor, QwixxRow> _rows;
    private int _penalties;

    public event EventHandler<ErrorEvent>? DecideError;

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
        return DecisionOrFallback(() => _thinker.DecideWhiteMark(gameState, whiteSum), null, "the white-dice sum is not marked");
    }

    // QX-010: delegates to the injected thinker. Only called for the active (rolling) player.
    public QwixxMark? DecideColoredMark(IQwixxReadOnlyGameState gameState, QwixxDiceRoll roll)
    {
        return DecisionOrFallback(() => _thinker.DecideColoredMark(gameState, roll), null, "no colored combination is marked");
    }

    // QX-022/QX-023: delegates to the injected thinker. Only called when the chosen mark
    // makes locking possible.
    public bool DecideToLock(IQwixxReadOnlyGameState gameState, QwixxColor color)
    {
        return DecisionOrFallback(() => _thinker.DecideToLock(gameState, color), false, "the row is left unlocked");
    }

    public IQwixxReadOnlyPlayer AsReadOnly()
    {
        return new QwixxReadOnlyPlayer(this);
    }

    // A thinker is hackathon-participant code, so every call into one is an untrusted boundary -
    // the same reason TakeFivePlayer.Decide wraps its own thinker calls. Each fallback is the
    // "do nothing" answer, which is always legal: declining a mark is a normal Qwixx move
    // (QX-009/QX-010) and never locks a row that the player didn't ask to lock (QX-023). The
    // active player still risks the QX-013 penalty for marking nothing, which is the fair
    // consequence of a thinker that can't answer. SetState is inside the try too, since a
    // participant may implement IQwixxThinker directly rather than extending BaseQwixxThinker.
    private TDecision DecisionOrFallback<TDecision>(Func<TDecision> decide, TDecision fallback, string fallbackDescription)
    {
        try
        {
            _thinker.SetState(AsReadOnly());
            return decide();
        }
        catch (Exception e)
        {
            var message = $"Decide Exception - Player {Name} could not decide, so {fallbackDescription}. Error message: {e.Message} ";
            DecideError?.Invoke(this, new(message));
            return fallback;
        }
    }
}
