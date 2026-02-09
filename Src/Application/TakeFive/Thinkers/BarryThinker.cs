using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.TakeFive;

namespace Application.TakeFive.Thinkers;

public class BarryThinker() : BaseTakeFiveThinker
{
    public override string Name => "Barry";

    private int round = 0;


    // [X][XX][X][XXXX]
    // [X]
    // [XX][X]
    // [X]
    // [X][X][X]

    // Tactic steps
    // play lowest value, then highest cowHeads
    // Push risky card
    // play low card when possible

    public override TakeFiveCard Decide(ITakeFiveReadOnlyGameState gameState)
    {
        round++;
        
        if (round %2 == 1)
        {
            return LowestCard();
        }
        else
        {
            return HighestValueCard();
        }
    }

    public override int Decide(IEnumerable<IEnumerable<TakeFiveCard>> cardsOnTable)
    {
        
        
        int rowCount = cardsOnTable.Count();
        return new Random().Next(rowCount - 1);
    }

    private TakeFiveCard LowestCard()
    {
        return _hand.OrderBy(c => c.Value).First();
    }

    private TakeFiveCard HighestValueCard()
    {
        
        return _hand.OrderBy(c => c.Value).OrderBy(c => c.CowHeads).Last();
    }

    private static int IndexOfRowWithLeastCowHeads(IEnumerable<IEnumerable<TakeFiveCard>> cardsOnTable)
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
