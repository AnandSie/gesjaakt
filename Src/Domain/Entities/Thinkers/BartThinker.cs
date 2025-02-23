using Domain.Entities.Players;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Thinkiers;

public class BartThinker : IThinker
{
    public TurnAction Decide(IGameStateReader gameState)
    {
        // Go Big - Go Long strategy 

        var tippingPointTake = 18;
        var tippingPointCoinsSelf = 10;
        if (gameState.AmountOfCoinsOnTable > tippingPointTake && gameState.PlayerOnTurn.CoinsAmount < tippingPointCoinsSelf)
        {
            return TurnAction.TAKECARD;
        }
        else
        {
            return TurnAction.SKIPWITHCOIN;
        }
    }
}
