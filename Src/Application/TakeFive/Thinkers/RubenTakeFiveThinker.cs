using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;
using Domain.Entities.Components;
using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.TakeFive;

namespace Application.TakeFive.Thinkers;

public class RubenTakeFiveThinker() : BaseTakeFiveThinker
{
    public override string Name => "Ruben";
    private bool isFirstRound = true;
    private int amountOfPlayers;

    // INSTRUCTIONS: 
    // 1. Rename this class to your own Thinker e.g. "HarryTakeFiveThinker"
    // 2. Give a cool name
    // 3. Implement these two methods. Read the documentation of each method (see the BaseThinker) for more details
    // 4. In `TakeFivePlayerFactory`, add your thinker
    // 5. Play 

    public override TakeFiveCard Decide(ITakeFiveReadOnlyGameState gameState)
    {
        if (isFirstRound)
        {
            isFirstRound=false;
            return LowestCard();
        }
        else
        {
            var safeCards = GetSafeCardsFromHand(gameState);
            if (safeCards.Any())
            {
                var highestSafeCard = safeCards
                .OrderByDescending(card => card.Value)
                .First();
                return highestSafeCard;
            } 
            else
            {
                return LowestCard();
            }
        }
    }

    public override int Decide(IEnumerable<IEnumerable<TakeFiveCard>> cardsOnTable)
    {
        return GetRowIndexWithLowestCowHeads(cardsOnTable);
    }

    private TakeFiveCard LowestCard()
    {
        return _hand.OrderBy(c => c.Value).First();
    }
    private static int GetLastValueFromRow(IEnumerable<TakeFiveCard> row)
    {
        return row.LastOrDefault()?.Value ?? 0;
    }

    private static int GetNumOfCardsInRow(IEnumerable<TakeFiveCard> row)
    {
        return row.Count();
    }

    private IEnumerable<int> GetSafeValuesForRow(IEnumerable<TakeFiveCard> row)
    {
        int start = GetLastValueFromRow(row) + 1;
        int count = Math.Max(0, 5 - GetNumOfCardsInRow(row));
        return Enumerable.Range(start, count);
    }

    private IEnumerable<int> GetSafeValues(ITakeFiveReadOnlyGameState gameState)
    {
        return gameState.CardRows.SelectMany(GetSafeValuesForRow).Distinct();
    }

    private IEnumerable<TakeFiveCard> GetSafeCardsFromHand(ITakeFiveReadOnlyGameState gameState)
    {
        var safeCards = new List<TakeFiveCard>();
        IEnumerable<int> safeValues = GetSafeValues(gameState);
        foreach (var card in _hand){
            if (safeValues.Contains(card.Value))
            {
                safeCards.Add(card);
            }}
        return safeCards;
    }

    private IEnumerable<int> GetCowHeadSums(IEnumerable<IEnumerable<TakeFiveCard>> cardRows)
    {
        return cardRows.Select(row => row.Sum(card => card.CowHeads));
    }
    
    private static int GetIndexOfMin(IEnumerable<int> ienumberable)
    {
        return ienumberable
        .Select((value, i) => new { value, i })
        .First(x => x.value == ienumberable.Min())
        .i;
    }

    private int GetRowIndexWithLowestCowHeads(IEnumerable<IEnumerable<TakeFiveCard>> cardRows)
    {
        return GetIndexOfMin(GetCowHeadSums(cardRows));
    }
}
