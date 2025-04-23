using Domain.Entities.Players;
using Domain.Interfaces;

namespace Domain.Entities.Thinkers;

public class MarijnThinker : IThinker
{
    public TurnAction Decide(IGameStateReader _)
    {
        return TurnAction.SKIPWITHCOIN;
    }
}