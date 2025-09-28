using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.TakeFive;

public interface ITakeFivePlayer : 
    INamed, 
    IDecide<ITakeFiveReadOnlyGameState, TakeFiveCard>, 
    IDecide<IEnumerable<IEnumerable<TakeFiveCard>>, int>,
    IToReadOnly<ITakeFiveReadOnlyPlayer>,
    ITakeFivePlayerActions
{
    public IReadOnlyCollection<TakeFiveCard> PenaltyCards { get; }

    public int CardsCount { get; }
}
