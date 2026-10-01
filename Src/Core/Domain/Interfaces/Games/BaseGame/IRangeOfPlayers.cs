namespace Domain.Interfaces.Games.BaseGame;

public interface IRangeOfPlayers
{
    public int MinNumberOfPlayers { get; }
    public int MaxNumberOfPlayers { get; }
}
