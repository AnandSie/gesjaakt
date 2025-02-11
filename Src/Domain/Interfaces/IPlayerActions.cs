using Domain.Entities.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces;

public interface IPlayerActions
{
    public void AcceptCoins(IEnumerable<ICoin> coins);
    public void AcceptCard(ICard card);
    public ICoin GiveCoin();
    public TurnAction Decide(IGameStateReader gameState);
}
