using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces;

namespace Domain.Entities.Cards;

public class Card : ICard
{
    private int _value;

    public Card(int v)
    {
        if (v <= 0)
        {
            throw new ArgumentException("value should be positive");
        }
        _value = v;
    }

    public int Value => _value;
}
