namespace Domain.Interfaces.Games.BaseGame;

// REFACTOR - add IGameState(zonder generic) zodat we niet e.g. IGesjaaktGameState heoven te maken => houdt simpler
public interface IGameState<TPlayer> where TPlayer : INamed
{
    public void AddPlayer(TPlayer newPlayer);

    public IEnumerable<TPlayer> Players { get; }

}   
