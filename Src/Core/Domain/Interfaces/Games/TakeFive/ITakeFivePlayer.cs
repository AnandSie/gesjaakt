using Domain.Entities.Events;
using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.BaseGame;
using System.Collections.Immutable;

namespace Domain.Interfaces.Games.TakeFive;

public interface ITakeFivePlayer :
    INamed,
    IDecide<ITakeFiveReadOnlyGameState, TakeFiveCard>,
    IDecide<ImmutableList<ImmutableList<TakeFiveCard>>, int>,
    IToReadOnly<ITakeFiveReadOnlyPlayer>,
    ITakeFivePlayerActions
{
    // REFACTOR (Consider) returning Points instead of the cards, as usually that is what you are interested in...
    public IReadOnlyCollection<TakeFiveCard> PenaltyCards { get; }

    public int CardsCount { get; }

    public event EventHandler<ErrorEvent>? DecideError;
    public event EventHandler<ErrorEvent>? CardNotFound;
}
