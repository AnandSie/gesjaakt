using Domain.Entities.Game.Qwixx;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.Qwixx;

public interface IQwixxPlayer : INamed, IScored, IToReadOnly<IQwixxReadOnlyPlayer>
{
    // QX-002: one row per color. Returns the same instance on every call for a given color.
    QwixxRow Row(QwixxColor color);

    // QX-018
    int Penalties { get; }
    void AddPenalty();

    // QX-020
    bool HasMaxPenalties { get; }

    // QX-009: called for every player, every turn.
    QwixxColor? DecideWhiteMark(IQwixxReadOnlyGameState gameState, int whiteSum);

    // QX-010: called only for the active (rolling) player.
    QwixxMark? DecideColoredMark(IQwixxReadOnlyGameState gameState, QwixxDiceRoll roll);

    // QX-022/QX-023: called only when the chosen mark makes locking possible.
    bool DecideToLock(IQwixxReadOnlyGameState gameState, QwixxColor color);
}
