using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.TakeFive;

public interface ITakeFivePlayer : INamed, IDecide<ITakeFiveReadOnlyGameState, TakeFiveCard>, IDecide<IEnumerable<IEnumerable<TakeFiveCard>>, int>, IToReadOnlyPlayer<ITakeFiveReadOnlyPlayer>
{
    // TODO: Move to ITakeFivePlayerActions (similar to gesjaakt)
    public void AccecptCards(IEnumerable<TakeFiveCard> cards);
    public void AccecptPenaltyCards(IEnumerable<TakeFiveCard> cards);

    public IReadOnlyCollection<TakeFiveCard> PenaltyCards { get; }

    public int CardsCount { get; }
}
