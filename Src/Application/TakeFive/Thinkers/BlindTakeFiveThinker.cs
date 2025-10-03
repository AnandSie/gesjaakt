using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.TakeFive;

namespace Application.TakeFive.Thinkers;

public class BlindTakeFiveThinker() : BaseTakeFiveThinker
{
    public override string Name => "Blind";

    public override TakeFiveCard Decide(ITakeFiveReadOnlyGameState gameState)
    {
        int cardCount = _hand.Count;
        var index = new Random().Next(cardCount - 1);
        return _hand[index];
    }

    public override int Decide(IEnumerable<IEnumerable<TakeFiveCard>> cardsOnTable)
    {
        int rowCount = cardsOnTable.Count();
        return new Random().Next(rowCount - 1);
    }
}
