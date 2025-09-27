using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.TakeFive;

public interface ITakeFiveThinker: IDecide<ITakeFiveReadOnlyGameState, int>, IDecide<IEnumerable<IEnumerable<TakeFiveCard>>, int>
{ }
