using Domain.Entities.Players;
using Domain.Interfaces;

namespace Domain.Entities.Thinkers;

public class JensThinker_R3 : IThinker
{
    private bool isMelking = false;

    public GesjaaktTurnOption Decide(IGameStateReader gameState)
    {
        var tippingPoint = 9;
        int nettoPenaltyPoints = gameState.OpenCardValue - gameState.AmountOfCoinsOnTable;
        var perceivedCoinValue = gameState.AmountOfCoinsOnTable * 2.9;
        if (gameState.AmountOfCoinsOnTable < 3)
        {
            return GesjaaktTurnOption.SKIPWITHCOIN;
        }
        if (nettoPenaltyPoints < tippingPoint || perceivedCoinValue > gameState.OpenCardValue)
        {
            return GesjaaktTurnOption.TAKECARD;
        }
        else if (IsInStraatjeAndLangGenoegGemolken(gameState))
        {
            return GesjaaktTurnOption.TAKECARD;
        }
        else
        {
            return GesjaaktTurnOption.SKIPWITHCOIN;
        }
    }

    private bool IsInStraatjeAndLangGenoegGemolken(IGameStateReader gameState)
    {
        foreach (var card in gameState.PlayerOnTurn.Cards)
        {
            var diff = Math.Abs(card.Value - gameState.OpenCardValue);
            if (diff <= 1)
            {
                if (isMelking)
                {
                    isMelking = false;
                    return true;
                }
                if (card.Value > 26)
                {
                    isMelking = true;
                    return false;
                }
                return true;
            }
        }
        return false;
    }
}