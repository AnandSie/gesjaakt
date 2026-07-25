using Application.Interfaces;
using Domain.Interfaces.Games.Qwixx;

namespace Application.Qwixx;

public class QwixxGameEventCollector(IGameEventHandler gameEventHandler) : IQwixxGameEventCollector
{
    public IQwixxGameEventCollector Attach(IQwixxGameDealer gamedealer)
    {
        gamedealer.ColorLocked += gameEventHandler.HandleEvent;
        gamedealer.PenaltyTaken += gameEventHandler.HandleEvent;
        gamedealer.MarkRejected += gameEventHandler.HandleEvent;
        return this;
    }

    public IQwixxGameEventCollector Attach(IEnumerable<IQwixxPlayer> players)
    {
        foreach (var player in players)
        {
            player.DecideError += gameEventHandler.HandleEvent;
        }
        return this;
    }
}
