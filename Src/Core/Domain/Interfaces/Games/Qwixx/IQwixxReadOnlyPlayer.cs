using Domain.Entities.Game.Qwixx;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.Qwixx;

public interface IQwixxReadOnlyPlayer : IReadOnlyPlayer, IScored
{
    int Penalties { get; }
    bool HasMaxPenalties { get; }

    // Query-only view of a row, deliberately not exposing QwixxRow itself: a Thinker only
    // ever sees this interface, and QwixxRow.Mark/Lock must stay reachable only through the
    // Dealer's own (mutable) IQwixxPlayer reference, not through game state handed to a bot.
    int MarkedCount(QwixxColor color);
    bool IsRowLocked(QwixxColor color);
    bool CanMark(QwixxColor color, int number);
}
