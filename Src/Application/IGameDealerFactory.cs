using Domain.Interfaces;

namespace Application
{
    public interface IGameDealerFactory
    {
        IGameDealer Create(IEnumerable<IPlayer> players);
    }
}
