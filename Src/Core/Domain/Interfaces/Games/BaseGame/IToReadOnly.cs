namespace Domain.Interfaces.Games.BaseGame;

public interface IToReadOnly<out T>
{
    public T AsReadOnly();
}