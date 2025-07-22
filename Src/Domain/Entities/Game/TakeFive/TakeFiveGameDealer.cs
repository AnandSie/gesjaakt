using Domain.Interfaces;
using Domain.Interfaces.Games.BaseGame;
using Domain.Interfaces.Games.Gesjaakt;
using Domain.Interfaces.Games.TakeFive;

namespace Domain.Entities.Game.TakeFive;

public class TakeFiveGameDealer : IGameDealer<ITakeFivePlayerState>
{
    public IEnumerable<ITakeFivePlayerState> GameResultOrdended()
    {
        throw new NotImplementedException();
    }

    public void Play()
    {
        throw new NotImplementedException();
    }

    public void Prepare()
    {
        throw new NotImplementedException();
    }

    public ITakeFivePlayerState Winner()
    {
        throw new NotImplementedException();
    }
}
