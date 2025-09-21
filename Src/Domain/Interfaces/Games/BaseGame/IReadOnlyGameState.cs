namespace Domain.Interfaces.Games.BaseGame;

public interface IReadOnlyGameState<out TPlayer> where TPlayer: INamed
{
    IEnumerable<TPlayer> Players { get; }
}
