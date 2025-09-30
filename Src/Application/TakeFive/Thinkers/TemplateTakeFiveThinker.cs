using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.TakeFive;

namespace Application.TakeFive.Thinkers;

public class TemplateTakeFiveThinker() : BaseTakeFiveThinker
{
    // INSTRUCTIONS: 
    // 1. Rename this class to your own Thinker e.g. "HarryTakeFiveThinker"
    // 2. Implement these two methods. Read the documentation of each method (see the BaseThinker) for more details
    // 3. In `TakeFivePlayerFactory`, add your thinker
    // 4. Play 

    public override int Decide(ITakeFiveReadOnlyGameState gameState)
    {
        throw new NotImplementedException();
    }

    public override int Decide(IEnumerable<IEnumerable<TakeFiveCard>> cardsOnTable)
    {
        // TODO: Consider implementing private method which calculates the row with the least points to give people headstart

        throw new NotImplementedException();
    }
}

// TODO: use/suggest - put them maybe in AbstractBaseThinker..

//public override int Decide(IEnumerable<IEnumerable<TakeFiveCard>> cardsOnTable)
//    {
//        return cardsOnTable
//            .Select((row, index) => new { Index = index, Sum = row.Sum(c => c.CowHeads) })
//            .OrderBy(x => x.Sum)
//            .First()
//            .Index;
//    }

//public override int Decide(IEnumerable<IEnumerable<TakeFiveCard>> cardsOnTable)
//    {
//        if (cardsOnTable == null || !cardsOnTable.Any())
//            throw new ArgumentException("cardsOnTable cannot be null or empty.");

//        int minIndex = -1;
//        int minSum = int.MaxValue;

//        int rowIndex = 0;
//        foreach (var row in cardsOnTable)
//        {
//            int sum = row.Sum(card => card.CowHeads);
//            if (sum < minSum)
//            {
//                minSum = sum;
//                minIndex = rowIndex;
//            }
//            rowIndex++;
//        }

//        return minIndex;
//    }
