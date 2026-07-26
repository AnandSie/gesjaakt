using Domain.Entities.Events;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.Gesjaakt;

public interface IGesjaaktGameDealer: IGameDealer<IGesjaaktPlayer>
{
    event EventHandler<SpecialEvent>? PlayerGesjaakt;
    event EventHandler<OrdinaryEvent>? SkippedWithCoin;
    event EventHandler<OrdinaryEvent>? CoinsDivided;
    event EventHandler<FaultEvent>? PlayerDecideError;
}
