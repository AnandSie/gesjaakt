using Domain.Interfaces.Games.Gesjaakt;
using Domain.Interfaces;

namespace Domain.Entities.Game.Gesjaakt;

public interface IGesjaaktGameState: IGesjaaktMutableGameState, IGesjaaktReadOnlyGameState
{
}