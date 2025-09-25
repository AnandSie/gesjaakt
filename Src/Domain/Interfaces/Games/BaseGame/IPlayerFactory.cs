namespace Domain.Interfaces.Games.BaseGame;

public interface IPlayerFactory<out TPlayer> where TPlayer : INamed
{
    public IEnumerable<TPlayer> Create();
    public IEnumerable<Func<TPlayer>> AllPlayerFactories();

    public TPlayer CreateHomoSapiens();
    public IEnumerable<TPlayer> CreateManualPlayers(int playersToAdd);

}
