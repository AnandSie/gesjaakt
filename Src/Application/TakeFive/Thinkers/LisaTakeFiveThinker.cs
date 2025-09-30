using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.TakeFive;

namespace Application.TakeFive.Thinkers;

public class LisaTakeFiveThinker() : BaseTakeFiveThinker
{
    private int roundsPlayed = 0;

    public override int Decide(ITakeFiveReadOnlyGameState gameState)
    {
        if (roundsPlayed == 0)
        {
            // TODO: dubbel check that both if/else are played

            var result = this._hand.OrderBy(c => c.Value).First().Value;
            roundsPlayed++;
            return result;
        }
        // Extra duiken (in eerste 4 beurten)
        else if (roundsPlayed < 5 && this._hand.Any(c => c.Value < 20))
        {
            roundsPlayed++;
            return this._hand.OrderBy(c => c.Value).First().Value;
        }
        else
        {
            // TODO: als er 5 kaarten, dan wil je grootste verschil spelen...

            var smallesRowValue = gameState.CardRows.Select(row => row.Count()).Order().First();

            var lastCardFromSmallestRows = gameState
                .CardRows
                .Where(row => row.Count() < TakeFiveRules.MaxCardsInRowAllowed)
                .Where(row => row.Count() == smallesRowValue)
                .Select(row => row.Last());

            // FIXME: fix empty possible loop
            var result = this._hand.Select(cardFromHand =>
                lastCardFromSmallestRows
                    .Where(cardOnTable => cardOnTable.Value < cardFromHand.Value)
                    .Select(cardOnTable => new
                    {
                        CardFromHand = cardFromHand,
                        Difference = cardFromHand.Value - cardOnTable.Value
                    })
                    .OrderBy(x => x.Difference).First()
            )
            .OrderBy(x => x.Difference)
            .First()
            .CardFromHand
            .Value;

            roundsPlayed++;
            return result;

            // TODO: what if empty
            // TODO: can we call events? (such that people can log what happens..)
        }
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
