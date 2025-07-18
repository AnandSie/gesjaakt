namespace Domain.Interfaces;

public interface IPlayerState
{
    string Name { get; }
    int CoinsAmount { get; }
    ICollection<ICard> Cards { get; }
    public int CardPoints();
    public int Points();
}
