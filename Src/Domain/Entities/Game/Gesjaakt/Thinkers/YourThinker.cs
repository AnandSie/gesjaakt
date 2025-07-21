using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces;

namespace Domain.Entities.Thinkers;

public class YourThinker : IThinker
{
    public GesjaaktTurnOption Decide(IGesjaaktReadOnlyGameState gameState)
    {
        // YourThinker
        if (true)
        {
            return GesjaaktTurnOption.TAKECARD;
        }
        else
        {
            return GesjaaktTurnOption.SKIPWITHCOIN;
        }
    }
}
