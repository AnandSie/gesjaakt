using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces.Components;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.Gesjaakt;

// FIXME: Composition > Inheritance : ReadOnlyAspect as seperate class from private method
public interface IGesjaaktPlayer : INamed, IDecide<IGesjaaktReadOnlyGameState, GesjaaktTurnOption>, IToReadOnlyPlayer<IGesjaaktReadOnlyPlayer>, IGesjaaktPlayerActions
{
    // TODO: DRY with the readonly version - maybe okay
    int CoinsAmount { get; }
    IEnumerable<ICard> Cards { get; }
    public int CardPoints();
    public int Points();
}
