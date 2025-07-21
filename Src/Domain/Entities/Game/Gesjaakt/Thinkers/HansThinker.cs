using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces;

namespace Domain.Entities.Thinkers;

public class HansThinker : IThinker
{
    public GesjaaktTurnOption Decide(IGesjaaktReadOnlyGameState gameState)
    {
        if (gameState.AmountOfCoinsOnTable >= (gameState.Players.Count()))
        {
            if ((gameState.OpenCardValue - gameState.AmountOfCoinsOnTable) < 20)
            {
                return GesjaaktTurnOption.TAKECARD;
            }
        }

        return GesjaaktTurnOption.SKIPWITHCOIN;
    }
}





