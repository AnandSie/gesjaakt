using Domain.Entities.Players;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Thinkiers;

public class GreedyThinker : IThinker
{
    public TurnAction Decide(IGameStateReader gameState)
    {
        return TurnAction.TAKECARD;
    }
}
