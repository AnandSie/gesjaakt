using Domain.Interfaces.Games.TakeFive;

namespace Application.Interfaces;

public interface ITakeFiveGameEventCollector
{
    public ITakeFiveGameEventCollector Attach(ITakeFiveGameState gameState);
    public ITakeFiveGameEventCollector Attach(ITakeFiveGameDealer gamedealer);
    public ITakeFiveGameEventCollector Attach(IEnumerable<ITakeFivePlayer> players);
}
