using Domain.Entities.Components;
using Domain.Interfaces.Components;

namespace Domain.Interfaces.Games.Gesjaakt;

public interface IGesjaaktPlayerActions
{
    public void AcceptCoins(IEnumerable<Coin> coins);
    public void AcceptCard(ICard card);
    public Coin GiveCoin();
}
