using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Components;

public interface IMutableDeck<TCard> where TCard : ICard
{
    void TakeOut(int amount);
    TCard DrawCard();
}
