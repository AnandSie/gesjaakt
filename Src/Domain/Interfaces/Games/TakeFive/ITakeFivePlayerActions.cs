using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.TakeFive;

// FIXME: misschien int vervangen door een enum lopen van 1 t/m 4 ..?
public interface ITakeFivePlayerActions : IPlayerActions<ITakeFiveReadOnlyGameState, int>
{
}
