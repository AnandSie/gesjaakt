using Domain.Entities.Players;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Thinkiers;

public class TemplateThinker : IThinker
{
    public TurnAction Decide(IGameStateReader _)
    {
        throw new NotImplementedException();
    }
}
