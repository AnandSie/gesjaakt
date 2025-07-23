using Domain.Interfaces.Components;
using Domain.Interfaces.Games.BaseGame;
using System.Collections.ObjectModel;

namespace Domain.Interfaces.Games.TakeFive;

public interface ITakeFivePlayerState: INamed
{
    public IReadOnlyCollection<ICard> PenaltyCards { get; }
}
