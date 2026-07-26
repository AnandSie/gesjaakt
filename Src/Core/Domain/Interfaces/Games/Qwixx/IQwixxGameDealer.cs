using Domain.Entities.Events;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.Qwixx;

public interface IQwixxGameDealer : IGameDealer<IQwixxPlayer>
{
    // QX-024/QX-025: a color just closed for every player, not only for the one who locked it.
    event EventHandler<NotableEvent>? ColorLocked;

    // QX-013/QX-014: the active player marked nothing this turn and took a penalty.
    event EventHandler<NotableEvent>? PenaltyTaken;

    // A thinker asked for a mark the rules don't allow. Silently dropping these is what made
    // unmarkable menu options so confusing to debug (see docs/qwixx/status-report.md).
    event EventHandler<FaultEvent>? MarkRejected;
}
