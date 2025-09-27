using Domain.Entities.Components;
using Domain.Interfaces;
using Domain.Interfaces.Components;
using Domain.Interfaces.Games.Gesjaakt;

namespace Domain.Entities.Game.Gesjaakt;

public class GesjaaktPlayer : IGesjaaktPlayer
{
    private readonly ICollection<Coin> _coins;
    private readonly ICollection<ICard> _cards;
    private readonly string? _name;
    private readonly IGesjaaktThinker _thinker;

    public GesjaaktPlayer(IGesjaaktThinker thinker, string? name = null)
    {
        _name = name;
        _coins = new HashSet<Coin>();
        _cards = new HashSet<ICard>();
        _thinker = thinker;
    }

    public GesjaaktTurnOption Decide(IGesjaaktReadOnlyGameState gameState)
    {
        return _thinker.Decide(gameState);
    }

    public int CoinsAmount => _coins.Count;

    public IEnumerable<ICard> Cards => _cards;

    public string Name => _name ?? "";

    public void AcceptCoins(IEnumerable<Coin> coins)
    {
        foreach (var coin in coins)
        {
            _coins.Add(coin);
        }
    }

    public Coin GiveCoin()
    {
        var coin = _coins.First();
        _coins.Remove(coin);
        return coin;
    }

    public void AcceptCard(ICard card)
    {
        _cards.Add(card);
        var cardValues = string.Join(", ", _cards.OrderBy(c => c.Value).Select(c => c.Value));
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

    public int Points()
    {
        return CardPoints() - CoinsAmount;
    }

    public IGesjaaktReadOnlyPlayer AsReadOnly()
    {
        return new GesjaaktReadOnlyPlayer(this);
    }

    public override string ToString()
    {
        return $"{_name ?? "unkown"}, has {Points()} penalty points, cards [{string.Join(", ", Cards.Select(c => c.Value).OrderBy(c => c))}] and {CoinsAmount} coins";
    }
}
