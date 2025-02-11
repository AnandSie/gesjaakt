using Domain.Entities.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces;

public interface IDecide
{
    public TurnAction Decide(IGameStateReader state);
}
