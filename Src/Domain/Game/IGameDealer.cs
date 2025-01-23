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
    void RemoveCardsFromDeck(int amount);

    [Obsolete("Should be private and replaced by Play")]
    void NextPlayerPlays();

    void Play();

    IPlayer Winner();
}
