using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces.Components;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.Gesjaakt;

public interface IGesjaaktPlayer : INamed, IDecide<IGesjaaktReadOnlyGameState, GesjaaktTurnOption>, IToReadOnlyPlayer<IGesjaaktReadOnlyPlayer>, IGesjaaktPlayerActions
{
    int CoinsAmount { get; }
    IEnumerable<ICard> Cards { get; }
    public int CardPoints();
    public int Points();

    // Events

}
