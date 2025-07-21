using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces.Components;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.Gesjaakt;

public interface IGesjaaktPlayerActions: IPlayerActions<IGesjaaktReadOnlyGameState, GesjaaktTurnOption>
{
    public void AcceptCoins(IEnumerable<ICoin> coins);
    public void AcceptCard(ICard card);
    public ICoin GiveCoin();
}
