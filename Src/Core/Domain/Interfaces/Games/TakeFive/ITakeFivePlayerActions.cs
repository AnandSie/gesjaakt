using Domain.Entities.Game.TakeFive;

namespace Domain.Interfaces.Games.TakeFive;

public interface ITakeFivePlayerActions
{
    public void AccecptCards(IEnumerable<TakeFiveCard> cards);
    public void AcceptsPenaltyCards(IEnumerable<TakeFiveCard> cards);
}
