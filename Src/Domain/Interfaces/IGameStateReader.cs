using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces;

public interface IGameStateReader
{
    IEnumerable<IPlayerState> Players { get; }
    IPlayerState PlayerOnTurn { get; }
    int OpenCardValue { get; }
    bool HasOpenCard {  get; }
    IDeckState Deck { get; }
    int AmountOfCoinsOnTable { get; }
}
