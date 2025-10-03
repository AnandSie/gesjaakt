using Domain.Entities.Components;
using Domain.Interfaces.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Games.BaseGame;

public interface ICardFactory<out TCard> where TCard: ICard
{
    TCard Create(int value);
}
