using Domain.Interfaces.Components;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.Gesjaakt;

public interface IGesjaaktPlayerState : INamed
{
    int CoinsAmount { get; }
    ICollection<ICard> Cards { get; }
    public int CardPoints();
    public int Points();
}
