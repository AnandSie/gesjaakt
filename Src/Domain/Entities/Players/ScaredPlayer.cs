using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Players;

public class ScaredPlayer : Player
{
    public ScaredPlayer() : base()
    {
        // TODO: Create a decider object and insert this into the player base
    }

    public override TurnAction Decide(IGameStateReader gameState)
    {
        return TurnAction.SKIPWITHCOIN;
    }
}
