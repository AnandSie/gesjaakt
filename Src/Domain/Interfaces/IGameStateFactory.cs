using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces.Games.Gesjaakt;

namespace Domain.Interfaces;

public interface IGameStateFactory
{
    IGesjaaktGameState Create(IEnumerable<IGesjaaktPlayer> players);
}
