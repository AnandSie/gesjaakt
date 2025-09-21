using Domain.Interfaces.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Components;

public class ReadOnlyDeck<TCard> : IReadOnlyDeck<TCard> where TCard : ICard
{
    private readonly IMutableDeck<TCard> _inner;

    public ReadOnlyDeck(IMutableDeck<TCard> inner)
    {
        _inner = inner;
    }

    public int AmountOfCardsLeft()
    {
        return _inner.AmountOfCardsLeft();
    }

    public bool IsEmpty()
    {
        return _inner.IsEmpty();
    }
}
