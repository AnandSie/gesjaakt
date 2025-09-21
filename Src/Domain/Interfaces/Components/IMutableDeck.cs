using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Components;

// TODO: make this interface internally accessible such that only relevant buisness logic can use it
public interface IMutableDeck<TCard>: IReadOnlyDeck<TCard> where TCard : ICard
{
    void TakeOut(int amount);
    TCard DrawCard();
}
