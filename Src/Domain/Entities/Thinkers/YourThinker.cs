using Domain.Entities.Players;
using Domain.Interfaces;

namespace Domain.Entities.Thinkers;

public class YourThinker : IThinker
{
    public GesjaaktTurnOption Decide(IGameStateReader gameState)
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
