using Domain.Interfaces.Components;
using Domain.Interfaces.Games.TakeFive;

namespace Domain.Entities.Game.TakeFive;

public class TakeFivePlayer : ITakeFivePlayer
{
    public string Name => throw new NotImplementedException();

    public IReadOnlyCollection<ICard> PenaltyCards => throw new NotImplementedException();

    public void AccecptCards(IEnumerable<TakeFiveCard> cards)
    {
        throw new NotImplementedException();
    }

    public void AccecptPenaltyCards(IEnumerable<TakeFiveCard> cards)
    {
        throw new NotImplementedException();
    }

    public int Decide(ITakeFiveReadOnlyGameState gameState)
    {
        throw new NotImplementedException();
    }
}
