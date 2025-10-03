using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.TakeFive;

public interface ITakeFiveReadOnlyGameState: IReadOnlyGameState<ITakeFiveReadOnlyPlayer>
{
    IEnumerable<IEnumerable<TakeFiveCard>> CardRows { get; }
}
