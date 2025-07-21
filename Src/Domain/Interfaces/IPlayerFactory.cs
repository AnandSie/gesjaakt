using Domain.Interfaces.Games.Gesjaakt;

namespace Domain.Interfaces;

public interface IPlayerFactory
{
    public IEnumerable<IGesjaaktPlayer> Create();
    public IEnumerable<Func<IGesjaaktPlayer>> AllPlayerFactories();

    public IGesjaaktPlayer CreateHomoSapiens(string name, IPlayerInputProvider playerInputProvider);
}
