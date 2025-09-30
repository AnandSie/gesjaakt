using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.TakeFive;

namespace Application.TakeFive.Thinkers;

public class LisaTakeFiveThinker() : BaseTakeFiveThinker
{
    private int round = 0;

    public override int Decide(ITakeFiveReadOnlyGameState gameState)
    {
        // Tactic steps
        // 1. First round play the lowest card
        // 2. In first four rounds, if there is a card lower than 20 play it
        // 3. Choose card that has smallest (positive) difference
        // 4. Only consider the row with the least cards

        // TODO:
        // 5. When you are forced to play in a row of five, play the card with the largest difference

        round++;
        // Step 1
        if (round == 1)
        {
            var result = LowestCard();
            return result;
        }
        // Step 2
        else if (round < 5 && _hand.Any(c => c.Value < 20))
        {
            return LowestCard();
        }
        else
        {
            // Step 4
            var rowSizeOfRowWithLeastCards = gameState.CardRows.Select(row => row.Count()).Order().First();
            
            var lastCardFromSmallestRows = gameState
                .CardRows
                // Step 4
                .Where(row => row.Count() == rowSizeOfRowWithLeastCards)
                .Select(row => row.Last());

            var cardsOrdedTactfully = _hand.Select(cardFromHand =>
                lastCardFromSmallestRows
                    // Step 3
                    .Where(cardOnTable => cardOnTable.Value < cardFromHand.Value)
                    .Select(cardOnTable => new
                    {
                        CardFromHand = cardFromHand,
                        // Step 3
                        Difference = cardFromHand.Value - cardOnTable.Value
                    })
                    // Step 3
                    .OrderBy(x => x.Difference)
                    .FirstOrDefault()
            )
            .Where(x => x != null)
            .OrderBy(x => x.Difference);

            // Default
            if (!cardsOrdedTactfully.Any())
            {
                // Default Option - highest/lowest does not matter (with only playing blind/diver)
                //return HighestCard(); 
                return LowestCard();
            }

            var result = cardsOrdedTactfully
                .First()
                .CardFromHand
                .Value;

            return result;
        }
    }

    private int LowestCard()
    {
        return _hand.OrderBy(c => c.Value).First().Value;
    }

    private int HighestCard()
    {
        return _hand.OrderBy(c => c.Value).Last().Value;
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
