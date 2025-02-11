using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Players;

// TODO: misschien abstract class Player maken
// => strategy pattern? 

// TODO: Inject IDecider 

public abstract class Player : IPlayer
{
    private readonly ICollection<ICoin> _coins;
    private readonly ICollection<ICard> _cards;
    private string? _name;
    public Player(string? name = null)
    {
        _name = name;
        _coins = new HashSet<ICoin>();
        _cards = new HashSet<ICard>();
    }

    public abstract TurnAction Decide(IGameStateReader gameState);

    public int CoinsAmount => _coins.Count;

    public ICollection<ICard> Cards => _cards;

    public string Name => _name ?? "";

    public void AcceptCoins(IEnumerable<ICoin> coins)
    {
        foreach (var coin in coins)
        {
            _coins.Add(coin);
        }
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

    public int CardPoints()
    {
        var sortedCards = _cards.OrderBy(card => card.Value).ToList();

        var lowestOfConnectedGroups = new List<ICard>();
        for (int i = 0; i < sortedCards.Count; i++)
        {
            if (i == 0 || sortedCards[i].Value != sortedCards[i - 1].Value + 1)
            {
                lowestOfConnectedGroups.Add(sortedCards[i]);
            }
        }

        return lowestOfConnectedGroups.Select(r => r.Value).Sum();
    }

    public override string ToString()
    {
        return $"{_name ?? "someone"} the {GetType().Name} has cards [{string.Join(", ", Cards.Select(c => c.Value).OrderBy(c => c))}] and {CoinsAmount} coins";
    }
}
