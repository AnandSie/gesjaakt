
using Domain.Entities.Components;
using Domain.Interfaces.Components;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Entities.Game.TakeFive;

public class TakeFiveDeckFactory : IDeckFactory<TakeFiveCard>
{
    private readonly ICardFactory<TakeFiveCard> _cardFactory;
    public TakeFiveDeckFactory(ICardFactory<TakeFiveCard> cardFactory)
    {
        _cardFactory = cardFactory;
    }

    public IMutableDeck<TakeFiveCard> Create()
    {
        return new Deck<TakeFiveCard>(TakeFiveRules.MinCardValue, TakeFiveRules.MaxCardValue, _cardFactory);
    }
}
