using Domain.Interfaces;

namespace Domain.Entities.Game.TakeFive;

public class TakeFiveGameDealer : IGameDealer
{
    public IEnumerable<IPlayer> GameResultOrdended()
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

    public IPlayerState Winner()
    {
        throw new NotImplementedException();
    }
}
