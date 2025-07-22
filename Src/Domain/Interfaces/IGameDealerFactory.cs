using Domain.Interfaces;
using Domain.Interfaces.Games.Gesjaakt;

namespace Application;

public interface IGameDealerFactory
{
    IGameDealer<IGesjaaktPlayerState> Create(IEnumerable<IGesjaaktPlayer> players);
}
