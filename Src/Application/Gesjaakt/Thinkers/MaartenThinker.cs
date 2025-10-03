using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces.Games.Gesjaakt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Gesjaakt.Thinkers;

public class MaartenThinker : IGesjaaktThinker
{
    public GesjaaktTurnOption Decide(IGesjaaktReadOnlyGameState gameState)
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
