using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Cards;
using Domain.Game;

namespace Domain.Players;

public interface IPlayer: IPlayerState, IDecide
{
    public void AcceptCoins(IEnumerable<ICoin> coins);
    public void AcceptCard(ICard card);
    public ICoin GiveCoin();
}
