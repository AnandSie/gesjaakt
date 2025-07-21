using Domain.Interfaces.Games.TakeFive;

namespace Domain.Entities.Game.TakeFive;

public class TakeFivePlayer : ITakeFivePlayer
{
    public string Name => throw new NotImplementedException();

    public int Decide(ITakeFiveReadOnlyGameState gameState)
    {
        throw new NotImplementedException();
    }
}
