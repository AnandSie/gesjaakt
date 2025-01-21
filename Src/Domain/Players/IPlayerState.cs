using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Cards;
using Domain.Game;

namespace Domain.Players;

public interface IPlayerState
{
    int CoinsAmount { get; }
    ICollection<ICard> Cards { get; }
}
