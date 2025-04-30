using Domain.Entities.Players;
using Domain.Interfaces;

namespace Domain.Entities.Thinkers;

public class YourThinker : IThinker
{
    public TurnAction Decide(IGameStateReader gameState)
    {
        // YourThinker
        if (true)
        {
            return TurnAction.TAKECARD;
        }
        else
        {
            return TurnAction.SKIPWITHCOIN;
        }
    }
}
