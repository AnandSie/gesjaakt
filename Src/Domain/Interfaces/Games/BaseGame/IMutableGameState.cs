namespace Domain.Interfaces.Games.BaseGame;

public interface IMutableGameState<TPlayer> where TPlayer : INamed
{
    public void AddPlayer(TPlayer newPlayer);
}   
