using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Cards;
using Domain.Players;

namespace Domain.Game;

public interface IGameStateReader
{
    IEnumerable<IPlayer> Players { get; }
    IPlayer PlayerOnTurn { get; }
    ICard OpenCard { get; }
    IDeck Deck { get; }
    int AmountOfCoinsOnTable { get; }
}
