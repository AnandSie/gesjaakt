namespace Domain.Interfaces.Games.BaseGame;

public interface IPlayerFactory<out TPlayer> where TPlayer : INamed
{
    public IEnumerable<TPlayer> Create();
    public IEnumerable<Func<TPlayer>> AllPlayerFactories();

    public TPlayer CreateManualPlayer();
    public IEnumerable<TPlayer> CreateManualPlayers(int playersToAdd);

}
