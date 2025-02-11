using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Players;

public class ScaredThinker : IThinker
{
    public TurnAction Decide(IGameStateReader _)
    {
        return TurnAction.SKIPWITHCOIN;
    }
}
