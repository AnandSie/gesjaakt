using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Cards;

public interface IDeck
{
    // TODO: SPlit into IDeckState, IDeckSetter
    void TakeOut(int amount);
    int AmountLeft();
    ICard DrawCard();
    bool IsEmpty();
}
