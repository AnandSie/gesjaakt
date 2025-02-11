using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces;

public interface IGameStateWriter
{
    public void AddPlayer(IPlayer player);
    public void AddCoinToTable(ICoin coin);
    public IEnumerable<ICoin> TakeCoins();
    public void RemoveCardsFromDeck(int amount);
    public void OpenNextCardFromDeck();
    public ICard TakeOpenCard();
    public void NextPlayer();
}