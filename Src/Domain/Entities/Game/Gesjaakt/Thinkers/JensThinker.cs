using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces;

namespace Domain.Entities.Thinkers;

public class JensThinker : IThinker
{
    private const int rondjesTippingPoint = 1;

    private bool isMelking = false;
    private int rondjes = 0;

    public GesjaaktTurnOption Decide(IGesjaaktReadOnlyGameState gameState)
    {
        var tippingPoint = 14;
        int nettoPenaltyPoints = gameState.OpenCardValue - gameState.AmountOfCoinsOnTable;
        var perceivedCoinValue = gameState.AmountOfCoinsOnTable * 2.7;

        if (nettoPenaltyPoints < tippingPoint || perceivedCoinValue > gameState.OpenCardValue)
        {
            return GesjaaktTurnOption.TAKECARD;
        }
        else if(IsInStraatjeAndLangGenoegGemolken(gameState))
        {
            return GesjaaktTurnOption.TAKECARD;
        }
        else
        {
            return GesjaaktTurnOption.SKIPWITHCOIN;
        }
    }

    private bool IsInStraatjeAndLangGenoegGemolken(IGesjaaktReadOnlyGameState gameState)
    {
        foreach (var card in gameState.PlayerOnTurn.Cards)
        {
            var diff = Math.Abs(card.Value - gameState.OpenCardValue);
            if (diff == 1)
            {
                // TODO: Extract to separete method
                if (isMelking && rondjes == rondjesTippingPoint)
                {
                    isMelking = false;
                    return true;
                }
                else
                {
                    rondjes += 1;
                    isMelking = true;
                }
            }
        }
        return false;
    }
}
