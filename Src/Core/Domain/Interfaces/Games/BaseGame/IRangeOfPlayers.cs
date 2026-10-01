namespace Domain.Interfaces.Games.BaseGame;

public interface IAmountOfPlayers
{
    public int MinNumberOfPlayers { get; }
    public int MaxNumberOfPlayers { get; }
}
