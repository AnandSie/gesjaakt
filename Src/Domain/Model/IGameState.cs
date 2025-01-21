using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model;

internal interface IGameState
{
    IPlayer[] Players { get; }
    ICard CardVisible { get; }
    IDeck Deck { get; }
    int CoinsOnTable { get; }
}
