using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.TakeFive;

public interface ITakeFiveReadOnlyPlayer: IReadOnlyPlayer
{
    public IReadOnlyCollection<TakeFiveCard> PenaltyCards { get; }

    public int CardsCount { get; }
}