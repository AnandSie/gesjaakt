using Domain.Interfaces;
using Domain.Interfaces.Games.Gesjaakt;

namespace Application;

public interface IGameDealerFactory
{
    IGameDealer<IGesjaaktReadOnlyPlayer> Create(IEnumerable<IGesjaaktPlayer> players);
}
