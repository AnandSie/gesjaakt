using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.TakeFive;

// TODO: Tweede decide nodig
// FIXME: misschien int vervangen door een enum lopen van 1 t/m 4 ..?
public interface ITakeFiveThinker: IDecide<ITakeFiveReadOnlyGameState, int>, IDecide<IEnumerable<IEnumerable<TakeFiveCard>>, int>
{ }
