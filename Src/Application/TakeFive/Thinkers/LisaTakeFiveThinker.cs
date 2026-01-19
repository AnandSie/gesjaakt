using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.TakeFive;

namespace Application.TakeFive.Thinkers;

public class LisaTakeFiveThinker() : BaseTakeFiveThinker
{
    private int round = 0;

    public override string Name => "Lisa";

    public override TakeFiveCard Decide(ITakeFiveReadOnlyGameState gameState)
    {
        round++;

        // Tactic steps
        // 1. First round play the lowest card
        // 2. In first four rounds, if there is a card lower than 20 play it
        // 3. Choose card that has smallest (positive) difference
        // 4. Only consider the row with the least cards

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
                return LowestCard();
            }

            var result = cardsOrdedTactfully
                .First()
                .CardFromHand;

            return result;
        }
    }

    private TakeFiveCard LowestCard()
    {
        return _hand.OrderBy(c => c.Value).First();
    }

    public override int Decide(IEnumerable<IEnumerable<TakeFiveCard>> cardsOnTable)
    {
        return IndexOfRowWithLeastCowHeads(cardsOnTable);
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
