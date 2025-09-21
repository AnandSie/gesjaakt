
using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.Gesjaakt;

public interface IGesjaaktThinker: IDecide<IGesjaaktReadOnlyGameState, GesjaaktTurnOption>
{}
