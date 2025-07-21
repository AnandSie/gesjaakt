using Domain.Interfaces;
using Domain.Interfaces.Games.Gesjaakt;

namespace Application
{
    public interface IGameDealerFactory
    {
        IGameDealer Create(IEnumerable<IGesjaaktPlayer> players);
    }
}
