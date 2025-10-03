using Domain.Entities.Events;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.Gesjaakt;

public interface IGesjaaktGameDealer: IGameDealer<IGesjaaktPlayer>
{
    event EventHandler<WarningEvent>? PlayerGesjaakt;
    event EventHandler<InfoEvent>? SkippedWithCoin;
    event EventHandler<InfoEvent>? CoinsDivided;
    event EventHandler<ErrorEvent>? PlayerDecideError;
}
