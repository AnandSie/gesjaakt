using Domain.Cards;
using Domain.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Players;

// TODO: misschien abstract class Player maken
// => strategy pattern? 

public abstract class Player : IPlayer
{
    private ICollection<ICoin> _coins;
    private ICollection<ICard> _cards;

    public Player()
    {
        _coins = new HashSet<ICoin>();
        _cards = new HashSet<ICard>();
    }

    public abstract TurnAction Decide(IGameStateReader gameState);

    public int CoinsAmount => _coins.Count();

    public ICollection<ICard> Cards => throw new NotImplementedException();

    public void AcceptCoins(IEnumerable<ICoin> coins)
    {
        _coins = coins.Concat(_coins).ToList();
    }

    public ICoin GiveCoin()
    {
        var coin = _coins.First();
        _coins.Remove(coin);
        return coin;
    }

    public void AcceptCard(ICard card)
    {
        _cards.Add(card);
    }
}
