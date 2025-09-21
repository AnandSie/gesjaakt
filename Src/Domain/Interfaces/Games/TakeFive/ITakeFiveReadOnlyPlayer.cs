using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.TakeFive;

public interface ITakeFiveReadOnlyPlayer: IReadOnlyPlayer
{
    // TODO: PenaltyCards en Cardscount zijn ook gedefinieerd in ITakeFivePlayer (voelt als DRY, echter wil ik seperation..., dus misschien moet het maar?)

    public IReadOnlyCollection<TakeFiveCard> PenaltyCards { get; }

    public int CardsCount { get; }
}