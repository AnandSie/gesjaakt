using Domain.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Game;

public interface IGameStateWriter
{
    public void AddPlayer(IPlayer player);
    public void AddCoinToTable(ICoin coin);
    public void NextPlayer();
}