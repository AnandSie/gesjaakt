using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.TakeFive;

namespace Application.TakeFive.Thinkers;

public class DiverTakeFiveThinker() : BaseTakeFiveThinker
{
    public override int Decide(ITakeFiveReadOnlyGameState gameState)
    {
        return _hand.OrderBy(c => c.Value).First().Value;
    }

    public override int Decide(IEnumerable<IEnumerable<TakeFiveCard>> cardsOnTable)
    {
        return cardsOnTable
            .Select((row, index) => new
            {
                Index = index,
                Sum = row.Sum(c => c.CowHeads)
            })
            .OrderBy(x => x.Sum)
            .First()
            .Index;
    }
}
