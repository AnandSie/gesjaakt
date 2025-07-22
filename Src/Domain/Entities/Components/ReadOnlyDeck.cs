using Domain.Interfaces.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Components;

public class ReadOnlyDeck: IDeckState
{
    private readonly Deck _inner;

    public ReadOnlyDeck(Deck inner)
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
