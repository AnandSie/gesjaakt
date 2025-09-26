namespace Domain.Interfaces.Games.BaseGame;

public interface IToReadOnlyPlayer<out T> where T : IReadOnlyPlayer
{
    public T AsReadOnly();
}