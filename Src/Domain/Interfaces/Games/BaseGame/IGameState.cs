namespace Domain.Interfaces.Games.BaseGame;

public interface IGameState<TPlayer> where TPlayer : INamed
{
    public void AddPlayer(TPlayer newPlayer);

    public IEnumerable<TPlayer> Players { get; }

}   
