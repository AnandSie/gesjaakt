using Domain.Entities.Players;

namespace Domain.Interfaces;

public interface IGesjaaktActions
{
    public void AcceptCoins(IEnumerable<ICoin> coins);
    public void AcceptCard(ICard card);
    public ICoin GiveCoin();
    public GesjaaktTurnOption Decide(IGameStateReader gameState);
}
