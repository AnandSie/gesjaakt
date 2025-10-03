using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces.Games.Gesjaakt;

namespace Application.Gesjaakt.Thinkers;

public class OliverThinker : IGesjaaktThinker
{

    public GesjaaktTurnOption Decide(IGesjaaktReadOnlyGameState gameState)
    {
        int test = 5;

        if (gameState.AmountOfCoinsOnTable > test && gameState.OpenCardValue < test * 4)
        {
            return GesjaaktTurnOption.TAKECARD;
        }
        else
        {
            return GesjaaktTurnOption.SKIPWITHCOIN;
        }
    }
}
