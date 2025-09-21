using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces.Components;

namespace Domain.Interfaces.Games.Gesjaakt;

public interface IGesjaaktPlayerActions
{
    public void AcceptCoins(IEnumerable<ICoin> coins);
    public void AcceptCard(ICard card);
    public ICoin GiveCoin();
}
