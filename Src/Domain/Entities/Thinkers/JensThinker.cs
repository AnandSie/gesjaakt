using Domain.Entities.Players;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Thinkiers;

public class JensThinker : IThinker
{
    public TurnAction Decide(IGameStateReader gameState)
    {
        var tippingPoint = 15;
        int nettoPenaltyPoints = gameState.OpenCardValue - gameState.AmountOfCoinsOnTable;
        var perceivedCoinValue = gameState.AmountOfCoinsOnTable * 3;

        if (nettoPenaltyPoints < tippingPoint || perceivedCoinValue > gameState.OpenCardValue)
        {
            return TurnAction.TAKECARD;
        }
        else
        {
            return TurnAction.SKIPWITHCOIN;
        }
    }
}
