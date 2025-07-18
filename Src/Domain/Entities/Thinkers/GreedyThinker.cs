using Domain.Entities.Players;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Thinkers;

public class GreedyThinker : IThinker
{
    public GesjaaktTurnOption Decide(IGameStateReader gameState)
    {
        if (gameState.AmountOfCoinsOnTable == 0)
        {
            return GesjaaktTurnOption.SKIPWITHCOIN;
        }
        return GesjaaktTurnOption.TAKECARD;
    }
}
