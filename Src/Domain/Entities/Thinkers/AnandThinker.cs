using Domain.Entities.Players;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Thinkers;

public class AnandThinker : IThinker
{
    public GesjaaktTurnOption Decide(IGameStateReader gameState)
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
