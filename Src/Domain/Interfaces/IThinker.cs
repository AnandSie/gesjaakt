﻿using Domain.Entities.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces;

public interface IThinker
{
    public TurnAction Decide(IGameStateReader gameState);
}
