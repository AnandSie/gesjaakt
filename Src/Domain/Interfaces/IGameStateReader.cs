using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces;

public interface IGameStateReader
{
    IEnumerable<IPlayer> Players { get; }
    IPlayer PlayerOnTurn { get; }
    int OpenCardValue { get; }
    IDeckState Deck { get; }
    int AmountOfCoinsOnTable { get; }
}
