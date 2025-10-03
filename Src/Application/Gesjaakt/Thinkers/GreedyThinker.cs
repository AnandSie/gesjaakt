using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces.Games.Gesjaakt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Gesjaakt.Thinkers;

public class GreedyThinker : IGesjaaktThinker
{
    public GesjaaktTurnOption Decide(IGesjaaktReadOnlyGameState gameState)
    {
        if (gameState.AmountOfCoinsOnTable == 0)
        {
            return GesjaaktTurnOption.SKIPWITHCOIN;
        }
        return GesjaaktTurnOption.TAKECARD;
    }
}
