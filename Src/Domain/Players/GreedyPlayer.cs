﻿using Domain.Cards;
using Domain.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Players;

public class GreedyPlayer : Player
{
    public GreedyPlayer() : base()
    {

    }

    public override TurnAction Decide(IGameStateReader gameState)
    {
        return TurnAction.TAKECARD;
    }
}
