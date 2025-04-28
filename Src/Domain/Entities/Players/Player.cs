using Domain.Interfaces;

namespace Domain.Entities.Players;

public class Player : IPlayer
{
    private readonly ICollection<ICoin> _coins;
    private readonly ICollection<ICard> _cards;
    private readonly string? _name;
    private readonly IThinker _thinker;
    private readonly ILogger<Player> _logger;

    public Player(IThinker thinker, ILogger<Player> logger, string? name = null)
    {
        _name = name;
        _coins = new HashSet<ICoin>();
        _cards = new HashSet<ICard>();
        _thinker = thinker;
        _logger = logger;
    }

    public TurnAction Decide(IGameStateReader gameState)
    {
        try
        {
            return _thinker.Decide(gameState);
        }
        catch (Exception e)
        {
            _logger.LogError("The calculation did not work");
            _logger.LogError(e.ToString());
            return TurnAction.SKIPWITHCOIN;
        }
    }

    public int CoinsAmount => _coins.Count;

    public ICollection<ICard> Cards => _cards;

    public string Name => _name ?? "";

    public void AcceptCoins(IEnumerable<ICoin> coins)
    {
        foreach (var coin in coins)
        {
            _coins.Add(coin);
        }
        _logger.LogDebug($"{_name} gains {coins.Count()} coins. New total amount: {_coins.Count}");
    }

    public ICoin GiveCoin()
    {
        var coin = _coins.First();
        _coins.Remove(coin);
        _logger.LogDebug($"{_name} plays a coin. Remaining coins: {_coins.Count}");
        return coin;
    }

    public void AcceptCard(ICard card)
    {
        _cards.Add(card);
        var cardValues = string.Join(", ", _cards.OrderBy(c => c.Value).Select(c => c.Value));
        _logger.LogDebug($"{_name} accepts card {card.Value}. Current cards: {cardValues}");
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

    public override string ToString()
    {
        return $"{_name ?? "unkown"}, has {Points()} penalty points, cards [{string.Join(", ", Cards.Select(c => c.Value).OrderBy(c => c))}] and {CoinsAmount} coins";
    }
}
