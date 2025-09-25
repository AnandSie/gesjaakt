using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.BaseGame;

namespace Application.TakeFive;

public class TakeFivePlayerFactory : IPlayerFactory<TakeFivePlayer>
{
    public IEnumerable<Func<TakeFivePlayer>> AllPlayerFactories()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<TakeFivePlayer> Create()
    {
        throw new NotImplementedException();
    }

    public TakeFivePlayer CreateHomoSapiens()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<TakeFivePlayer> CreateManualPlayers(int playersToAdd)
    {
        throw new NotImplementedException();
    }
}
