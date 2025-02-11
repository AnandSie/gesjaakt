using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces;

public interface IPlayer : IPlayerState, IDecide
{
    string Name { get; }
    public void AcceptCoins(IEnumerable<ICoin> coins);
    public void AcceptCard(ICard card);
    public ICoin GiveCoin();
}
