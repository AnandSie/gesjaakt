using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Thinkers;

public class BartThinker : IThinker
{
    public GesjaaktTurnOption Decide(IGesjaaktReadOnlyGameState gameState)
    {
        // Go Big - Go Long strategy 

        var tippingPointTake = 18;
        var tippingPointCoinsSelf = 10;
        if (gameState.AmountOfCoinsOnTable > tippingPointTake && gameState.PlayerOnTurn.CoinsAmount < tippingPointCoinsSelf)
        {
            return GesjaaktTurnOption.TAKECARD;
        }
        else
        {
            return GesjaaktTurnOption.SKIPWITHCOIN;
        }
    }
}
