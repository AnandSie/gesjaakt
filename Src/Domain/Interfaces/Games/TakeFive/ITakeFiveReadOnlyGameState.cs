using Domain.Interfaces.Components;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.TakeFive;

public interface ITakeFiveReadOnlyGameState: IReadOnlyGameState<ITakeFivePlayerState>
{
    IDeck Deck { get; } // Deck gebeurd eigenlijk niks mee..., alleen bij opstarten.

    IEnumerable<IEnumerable<ICard>> CardRows { get; }
}
