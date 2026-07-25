using Domain.Interfaces.Games.Qwixx;

namespace Application.Interfaces;

// No Attach(IQwixxGameState) overload, unlike the Gesjaakt/TakeFive collectors: QwixxGameState
// raises no events of its own - every notable moment in a Qwixx turn (a lock, a penalty, a
// rejected mark) is decided by the dealer, and thinker failures are reported by the player.
public interface IQwixxGameEventCollector
{
    public IQwixxGameEventCollector Attach(IQwixxGameDealer gamedealer);
    public IQwixxGameEventCollector Attach(IEnumerable<IQwixxPlayer> players);
}
