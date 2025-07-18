using Domain.Entities.Players;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Thinkers;

public class MaartenThinker : IThinker
{
    public GesjaaktTurnOption Decide(IGameStateReader gameState)
    {
        var perceivedCoinValue = gameState.AmountOfCoinsOnTable * 3; 
        if (perceivedCoinValue > gameState.OpenCardValue)
        {
            return GesjaaktTurnOption.TAKECARD;
        }
        else
        {
            return GesjaaktTurnOption.SKIPWITHCOIN;
        }
    }
}
