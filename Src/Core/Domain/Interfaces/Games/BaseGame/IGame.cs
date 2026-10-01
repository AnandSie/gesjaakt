namespace Domain.Interfaces.Games.BaseGame;

public interface IGame<TPlayer> : IAmountOfPlayers, IGame where TPlayer : INamed
{
    public void PlayWith(IEnumerable<TPlayer> players);
    public IOrderedEnumerable<TPlayer> Results();

}

public interface IGame
{
    public static string Name { get; }
}
