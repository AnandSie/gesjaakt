using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces;

public interface IGameStateWriter
{
    IEnumerable<IPlayerActions> Players { get; }
    public void AddPlayer(IPlayerActions player);
    public void AddCoinToTable(ICoin coin);
    public IEnumerable<ICoin> TakeCoins();
    public void RemoveCardsFromDeck(int amount);
    public void OpenNextCardFromDeck();
    public ICard TakeOpenCard();
    public void NextPlayer();
}