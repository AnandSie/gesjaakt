using Domain.Interfaces.Components;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.Gesjaakt;

public interface IGesjaaktReadOnlyPlayer : IReadOnlyPlayer
{
    int CoinsAmount { get; }
    IReadOnlyCollection<ICard> Cards { get; }
    public int CardPoints();
    public int Points();
}
