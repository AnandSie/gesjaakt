using Domain.Entities.Players;
using Domain.Interfaces;

namespace Domain.Entities.Thinkers;

public class HansThinker : IThinker
{
    public TurnAction Decide(IGameStateReader gameState)
    {
        if (gameState.AmountOfCoinsOnTable >= (gameState.Players.Count()))
        {
            if ((gameState.OpenCardValue - gameState.AmountOfCoinsOnTable) < 20)
            {
                return TurnAction.TAKECARD;
            }
        }

        return TurnAction.SKIPWITHCOIN;
    }
}





