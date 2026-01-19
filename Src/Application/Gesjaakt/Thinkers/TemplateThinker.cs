using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces.Games.Gesjaakt;

namespace Application.Gesjaakt.Thinkers;

public class TemplateThinker : IGesjaaktThinker
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
