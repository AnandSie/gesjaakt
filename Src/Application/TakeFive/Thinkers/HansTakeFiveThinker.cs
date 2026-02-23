using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.TakeFive;

namespace Application.TakeFive.Thinkers;

public class HansTakeFiveThinker : BaseTakeFiveThinker
{
    public override string Name => "Hans";

    public override TakeFiveCard Decide(ITakeFiveReadOnlyGameState gameState)
    {
        // Which cards are safe to play?
        var safeCardValues = GetSafeCards(gameState);
        var safeCardsInHand = _hand
            .Where(c => safeCardValues.Contains(c.Value))
            .OrderBy(c => c.Value)
            .ToList();
        if (safeCardsInHand.Count > 0)
            return safeCardsInHand[^1];

        // Which cards are reasonably safe to play?
        var almostSafeCardValues = GetAlmostSafeCards(gameState);
        var almostSafeCardsInHand = _hand
            .Where(c => almostSafeCardValues.Contains(c.Value))
            .OrderBy(c => c.Value)
            .ToList();
        if (almostSafeCardsInHand.Count > 0)
            return almostSafeCardsInHand[^1];

        // If we have many cards, play the highest
        if (_hand.Count >= 9)
            return HighestCard();

        // If a row has few cow heads, play the lowest card
        if (LowestNumberOfCowHeads(gameState.CardRows) <= 2)
            return LowestCard();

        // Otherwise, play a random card
        var index = Random.Shared.Next(_hand.Count);
        return _hand[index];
    }

    public override int Decide(IEnumerable<IEnumerable<TakeFiveCard>> cardsOnTable)
    {
        return IndexOfRowWithLeastCowHeads(cardsOnTable);
    }

    private TakeFiveCard HighestCard()
    {
        return _hand.OrderBy(c => c.Value).Last();
    }

    private TakeFiveCard LowestCard()
    {
        return _hand.OrderBy(c => c.Value).First();
    }

    private static int LowestNumberOfCowHeads(IEnumerable<IEnumerable<TakeFiveCard>> cardsOnTable)
    {
        return cardsOnTable
            .Select((row, index) => new
            {
                Index = index,
                Sum = row.Sum(c => c.CowHeads)
            })
            .OrderBy(x => x.Sum)
            .First()
            .Sum;
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

    private static List<int> GetSafeCards(ITakeFiveReadOnlyGameState gameState)
    {
        List<int> safeCards = new();

        foreach (var row in gameState.CardRows)
        {
            // Number of open slots in the row
            var openSlots = 5 - row.Count();
            var lastCardValue = row.Last().Value;
            
            if (openSlots <= 0)
                continue;
            
            safeCards.AddRange(
                Enumerable.Range(lastCardValue + 1, openSlots)
            );
        }

        return safeCards;
    }

    private static List<int> GetAlmostSafeCards(ITakeFiveReadOnlyGameState gameState)
    {
        List<int> almostSafeCards = new();
        int playerCount = gameState.Players.Count();

        foreach (var row in gameState.CardRows)
        {
            // Offset depends on number of players
            var offset = Math.Max(0, 6 - playerCount);

            // Number of open slots in the row
            var openSlots = 5 - row.Count();
            var lastCardValue = row.Last().Value;
            
            if (openSlots <= 0)
                continue;
            almostSafeCards.AddRange(
                Enumerable.Range(lastCardValue + 1, openSlots + offset)
            );
        }

        return almostSafeCards;
    }
}
