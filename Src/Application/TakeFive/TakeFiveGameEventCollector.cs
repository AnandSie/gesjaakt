using Application.Interfaces;
using Domain.Interfaces.Games.TakeFive;

namespace Application.TakeFive;

public class TakeFiveGameEventCollector(IGameEventHandler gameEventHandler) : ITakeFiveGameEventCollector
{
    public ITakeFiveGameEventCollector Attach(ITakeFiveGameState gameState)
    {
        gameState.CardIsPlaced += gameEventHandler.HandleEvent;
        gameState.RowIsTaken += gameEventHandler.HandleEvent;
        return this;
    }

    public ITakeFiveGameEventCollector Attach(ITakeFiveGameDealer gamedealer)
    {
        // TODO
        //throw new NotImplementedException();
        return this;
    }

    public ITakeFiveGameEventCollector Attach(IEnumerable<ITakeFivePlayer> players)
    {
        foreach (var player in players)
        {
            player.DecideError += gameEventHandler.HandleEvent;
            player.CardNotFound += gameEventHandler.HandleEvent;
        }
        return this;
    }
}
