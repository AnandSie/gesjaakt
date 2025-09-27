using Domain.Interfaces.Games.BaseGame;
using Domain.Interfaces.Games.TakeFive;

namespace Application.TakeFive;

public class TakeFivePlayerFactory : IPlayerFactory<ITakeFivePlayer>
{
    public IEnumerable<Func<ITakeFivePlayer>> AllPlayerFactories()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ITakeFivePlayer> Create()
    {
        throw new NotImplementedException();
    }

    public ITakeFivePlayer CreateManualPlayer()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ITakeFivePlayer> CreateManualPlayers(int playersToAdd)
    {
        throw new NotImplementedException();
    }
}
