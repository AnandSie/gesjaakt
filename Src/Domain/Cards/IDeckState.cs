using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Cards;

public interface IDeckState
{
    int AmountLeft();
    ICard DrawCard();
    bool IsEmpty();
}
