using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces.Games.Gesjaakt;

namespace Application.Gesjaakt.Thinkers;

public class HansThinker : IGesjaaktThinker
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





