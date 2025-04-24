using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces;
using Extensions;

namespace Domain.Entities.Cards;

public class Deck : IDeck
{
    private readonly ICollection<ICard> Cards;

    public Deck(int min, int max)
    {
        int deckSize = max - min + 1;
        Cards = Enumerable.Range(min, deckSize)
                               .Select(value => new Card(value))
                               .Shuffle()
                               .ToHashSet<ICard>();
    }

    public int AmountOfCardsLeft()
    {
        return Cards.Count;
    }

    public ICard DrawCard()
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
