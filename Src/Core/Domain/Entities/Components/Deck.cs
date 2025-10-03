using Domain.Interfaces.Components;
using Domain.Interfaces.Games.BaseGame;
using Extensions;

namespace Domain.Entities.Components;

public class Deck<TCard> : IMutableDeck<TCard> where TCard : ICard
{
    private readonly List<TCard> Cards;

    public Deck(int min, int max, ICardFactory<TCard> cardFactory)
    {
        int deckSize = max - min + 1;
        Cards = Enumerable.Range(min, deckSize)
                               .Select(cardFactory.Create)
                               .Shuffle()
                               .ToList();
    }

    public IReadOnlyDeck<TCard> AsReadOnly() => new ReadOnlyDeck<TCard>(this);

    public int AmountOfCardsLeft()
    {
        return Cards.Count;
    }

    public TCard DrawCard()
    {
        var card = Cards.First();
        Cards.Remove(card);
        return card;
    }

    public bool IsEmpty()
    {
        return Cards.Count == 0;
    }

    public void TakeOut(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            var cardToRemove = Cards.First();
            Cards.Remove(cardToRemove);
        }
    }
}
