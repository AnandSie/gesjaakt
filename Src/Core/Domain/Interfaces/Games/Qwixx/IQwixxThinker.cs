using Domain.Entities.Game.Qwixx;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.Qwixx;

public interface IQwixxThinker : INamed
{
    // QX-009: called for every player, every turn - decide which row (if any) to mark
    // using the fixed white-dice sum.
    QwixxColor? DecideWhiteMark(IQwixxReadOnlyGameState gameState, int whiteSum);

    // QX-010: called only for the active (rolling) player - decide which one of the
    // candidate colored sums (if any) to mark.
    QwixxMark? DecideColoredMark(IQwixxReadOnlyGameState gameState, QwixxDiceRoll roll);

    // QX-022/QX-023: called only when the mark just chosen (white or colored) is a row's last
    // number AND the 5-mark lock threshold would be met - locking is a real choice even when
    // eligible, not automatic.
    bool DecideToLock(IQwixxReadOnlyGameState gameState, QwixxColor color);
}
