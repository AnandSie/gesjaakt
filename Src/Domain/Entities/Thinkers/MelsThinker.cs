using Domain.Entities.Players;
using Domain.Interfaces;

namespace Domain.Entities.Thinkers;

public class MelsThinker : IThinker
{
    public GesjaaktTurnOption Decide(IGameStateReader gameState)
    {
        // YourThinker
        if (gameState.AmountOfCoinsOnTable < 3 && gameState.PlayerOnTurn.CoinsAmount > 3)
        {
            return GesjaaktTurnOption.SKIPWITHCOIN;
        }
        else if (CreatesStreetThatIsWorth(gameState))
        {
            return GesjaaktTurnOption.TAKECARD;
        }
        else if (IsCardWorthIt(gameState))
        {
            return GesjaaktTurnOption.TAKECARD;
        }
        else
        {
            return GesjaaktTurnOption.SKIPWITHCOIN;
        }
    }

    public bool moreAdvancedLogic(IGameStateReader gameState)
    {

        return true;
    }

    private static bool CreatesStreetThatIsWorth(IGameStateReader gameState)
    {
        var createsStreet = gameState.PlayerOnTurn.Cards.Any(c => Math.Abs(c.Value - gameState.OpenCardValue) == 1);
        if (CalcCardRatio(gameState.OpenCardValue, gameState.AmountOfCoinsOnTable) < 3)
        {
            return true;
        }
        //if (gameState.PlayerOnTurn.CoinsAmount < gameState.AmountOfCoinsOnTable && gameState.AmountOfCoinsOnTable >= gameState.Players.Count()) { 
        //    return true; 
        //}
        return false;
    }

    private static bool IsCardWorthIt(IGameStateReader gameState)
    {
        return CalcCardRatio(gameState.OpenCardValue, gameState.AmountOfCoinsOnTable) < 1.5;
    }

    private static float CalcCardRatio(int cardValue, int coinsOnTable)
    {
        if (coinsOnTable == 0) { return int.MaxValue; }
        return cardValue / coinsOnTable;
    }
}