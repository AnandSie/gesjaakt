using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces.Components;

namespace Domain.Entities.Components;

public class Card : ICard
{
    private int _value;

    public Card(int value)
    {
        if (value <= 0)
        {
            throw new ArgumentException("value should be positive");
        }
        _value = value;
    }

    public int Value => _value;
}
