using Domain.Entities.Game;
using Domain.Entities.Players;
using Domain.Interfaces;

namespace Domain.Entities.Thinkers;

public class BarryThinker : IThinker
{
    int hi;
    int low;
    
    public TurnAction Decide(IGameStateReader gameState)
    {
        
        
        if ( gameState.PlayerOnTurn.Cards.Count == 0)
        {
            hi = gameState.OpenCardValue;
            low = gameState.OpenCardValue;
            return TurnAction.TAKECARD;
        }
        if (gameState.OpenCardValue > low - 3)
        {
            low = gameState.OpenCardValue;
            return TurnAction.TAKECARD;
        }
        else if (gameState.OpenCardValue < hi + 2)
        {
            hi = gameState.OpenCardValue;
            return TurnAction.TAKECARD;
        }
        else
        {
            return TurnAction.SKIPWITHCOIN;
        }

    }
}