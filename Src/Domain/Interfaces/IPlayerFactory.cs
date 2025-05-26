namespace Domain.Interfaces;

public interface IPlayerFactory
{
    public IEnumerable<IPlayer> Create();
    public IEnumerable<Func<IPlayer>> AllPlayerFactories();

    public IPlayer CreateHomoSapiens(string name, IPlayerInputProvider playerInputProvider);
}
