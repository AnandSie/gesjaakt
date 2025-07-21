using Domain.Interfaces;
using Domain.Interfaces.Games.Gesjaakt;

namespace Domain.Entities.Game.TakeFive;

public class TakeFiveGameDealer : IGameDealer
{
    public IEnumerable<IGesjaaktPlayerState> GameResultOrdended()
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

    public IGesjaaktPlayerState Winner()
    {
        throw new NotImplementedException();
    }
}
