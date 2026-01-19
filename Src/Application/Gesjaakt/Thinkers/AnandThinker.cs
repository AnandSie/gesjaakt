using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces.Games.Gesjaakt;

namespace Application.Gesjaakt.Thinkers;

public class AnandThinker : IGesjaaktThinker
{
    public GesjaaktTurnOption Decide(IGesjaaktReadOnlyGameState gameState)
    {
        var tippingPoint = 15;
        int nettoPenaltyPoints = gameState.OpenCardValue - gameState.AmountOfCoinsOnTable;
        if (nettoPenaltyPoints < tippingPoint)
        {
            return GesjaaktTurnOption.TAKECARD;
        }
        else
        {
            return GesjaaktTurnOption.SKIPWITHCOIN;
        }
    }
}
