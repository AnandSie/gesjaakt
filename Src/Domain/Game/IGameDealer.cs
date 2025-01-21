using Domain.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Game;

public interface IGameDealer
{
    IGameStateReader State { get; }
    void AddPlayer(IPlayer player);
    void DivideCoins(int coinsAmount);
    void NextPlayerPlays();
    void CalculateEndScore();
}
