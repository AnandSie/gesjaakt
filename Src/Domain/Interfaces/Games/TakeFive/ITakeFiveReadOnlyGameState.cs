using Domain.Interfaces.Components;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.TakeFive;

public interface ITakeFiveReadOnlyGameState: IReadOnlyGameState<ITakeFivePlayerState>
{
    IDeckState Deck { get; } // Deck gebeurd eigenlijk niks mee..., alleen bij opstarten.
    // TODO: ensure it is realy readonly 

    IEnumerable<IEnumerable<ICard>> CardRows { get; }
}
