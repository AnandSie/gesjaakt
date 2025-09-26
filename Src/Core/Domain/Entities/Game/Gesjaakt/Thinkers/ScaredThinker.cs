using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces.Games.Gesjaakt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Thinkers;

public class ScaredThinker : IGesjaaktThinker
{
    public GesjaaktTurnOption Decide(IGesjaaktReadOnlyGameState _)
    {
        return GesjaaktTurnOption.SKIPWITHCOIN;
    }
}
