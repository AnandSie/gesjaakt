using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.TakeFive;

namespace Application.TakeFive.Thinkers;

public class TemplateTakeFiveThinker() : BaseTakeFiveThinker
{
    // INSTRUCTIONS: 
    // 1. Rename this class to your own Thinker e.g. "HarryTakeFiveThinker"
    // 2. Implement these two methods. Read the documentation of each method (see the BaseThinker) for more details
    // 3. In `TakeFivePlayerFactory`, add your thinker
    // 4. Play 

    public override TakeFiveCard Decide(ITakeFiveReadOnlyGameState gameState)
    {
        throw new NotImplementedException();
    }

    public override int Decide(IEnumerable<IEnumerable<TakeFiveCard>> cardsOnTable)
    {
        throw new NotImplementedException();
    }
}
