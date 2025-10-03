using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.TakeFive;

namespace Application.TakeFive.Thinkers;

public class TemplateTakeFiveThinker() : BaseTakeFiveThinker
{
    public override string Name => throw new NotImplementedException();

    // INSTRUCTIONS: 
    // 1. Rename this class to your own Thinker e.g. "HarryTakeFiveThinker"
    // 2. Give a cool name
    // 3. Implement these two methods. Read the documentation of each method (see the BaseThinker) for more details
    // 4. In `TakeFivePlayerFactory`, add your thinker
    // 5. Play 

    public override TakeFiveCard Decide(ITakeFiveReadOnlyGameState gameState)
    {
        throw new NotImplementedException();
    }

    public override int Decide(IEnumerable<IEnumerable<TakeFiveCard>> cardsOnTable)
    {
        throw new NotImplementedException();
    }
}
