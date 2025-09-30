using Application.Interfaces;
using Domain.Interfaces.Games.TakeFive;

namespace Application.TakeFive;

public class TakeFiveGameEventCollector(IGameEventHandler gameEventHandler) : ITakeFiveGameEventCollector
{
    public ITakeFiveGameEventCollector Attach(ITakeFiveGameState gameState)
    {
        throw new NotImplementedException();
    }

    public ITakeFiveGameEventCollector Attach(ITakeFiveGameDealer gamedealer)
    {
        throw new NotImplementedException();
    }

    public ITakeFiveGameEventCollector Attach(IEnumerable<ITakeFivePlayer> players)
    {
        foreach (var player in players)
        {
            player.DecideError += gameEventHandler.HandleEvent;
        }
        return this;
    }
}
