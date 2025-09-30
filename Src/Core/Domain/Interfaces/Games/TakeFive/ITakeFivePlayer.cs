using Domain.Entities.Events;
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
    // REFACTOR (Consider) returning Points instead of the cards, as usually that is what you are interested in...
    public IReadOnlyCollection<TakeFiveCard> PenaltyCards { get; }

    public int CardsCount { get; }

    public event EventHandler<ErrorEvent>? DecideError;

}
