using Domain.Entities.Components;
using Domain.Entities.Events;
using Domain.Entities.Game.BaseGame;
using Domain.Interfaces.Components;
using Domain.Interfaces.Games.Gesjaakt;
using System.Text;

namespace Domain.Entities.Game.Gesjaakt;

// TODO: granada hotel verder gaan - draai de gamedealerfactory terug (smell met logger in domain objects even behouden), dat refactoren we later. 
// We willen het nu gewoon even werkend krijgen/houden

public class GesjaaktGameState : IGesjaaktGameState
{
    private readonly IList<IGesjaaktPlayer> _players;
    private int _playerIndex;
    private ICollection<ICoin> _coinsOnTable;
    private readonly Deck<Card> _deck;
    private ICard? _openCard;

    public event EventHandler<InfoEvent>? CardDrawnFromDeck;

    public GesjaaktGameState()
    {
        _players = new List<IGesjaaktPlayer>();
        _playerIndex = 0;
        _coinsOnTable = new HashSet<ICoin>();
        var factory = new CardFactory(); // TODO: DI - Gesjaakt Di
        _deck = new Deck<Card>(3, 35, factory); // TODO: extract to config
    }

    // TODO: Custom Exception
    public int OpenCardValue => _openCard?.Value ?? throw new Exception("There is no card yet");

    public bool HasOpenCard => _openCard != null;

    // FIXME: ensure it is really readonly? only casting is not sufficient?
    public IReadOnlyDeck<Card> Deck => new ReadOnlyDeck<Card>(_deck);

    public int AmountOfCoinsOnTable => _coinsOnTable.Count();

    public IGesjaaktPlayer PlayerOnTurn => _players[_playerIndex];

    public IEnumerable<IGesjaaktPlayer> Players => _players;

    public void OpenNextCardFromDeck()
    {
        _openCard = _deck.DrawCard();

        string message = $"Card drawn: {_openCard.Value}. Cards left: {Deck.AmountOfCardsLeft()}";
        CardDrawnFromDeck?.Invoke(this, new(message));
    }

    public ICard TakeOpenCard()
    {
        if (_openCard is null)
        {
            // TODO: Custom Exception
            throw new Exception("There is no open card to give");
        }

        var result = _openCard;
        _openCard = null;
        return result;
    }

    public void AddCoinToTable(ICoin coin)
    {
        _coinsOnTable.Add(coin);
    }

    public void AddPlayer(IGesjaaktPlayer newPlayer)
    {
        _players.Add(newPlayer);
    }

    public void NextPlayer()
    {
        if (_playerIndex == _players.Count - 1)
        {
            _playerIndex = 0;
        }
        else
        {
            _playerIndex++;
        }
    }

    public void RemoveCardsFromDeck(int amount)
    {
        _deck.TakeOut(amount);
    }

    public IEnumerable<ICoin> TakeCoins()
    {
        var result = _coinsOnTable;
        _coinsOnTable = new HashSet<ICoin>();
        return result;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine("Player states:");
        foreach (var player in _players)
        {
            sb.AppendLine($"- {player.ToString()}");
        }
        sb.AppendLine($"Card open: {OpenCardValue}");
        sb.AppendLine($"Coins On Table: {AmountOfCoinsOnTable}");
        sb.AppendLine($"Cards Left: {Deck.AmountOfCardsLeft()}");

        return sb.ToString();
    }

    public void DivideCoins(int coinsPerPlayer)
    {
        foreach (var player in _players)
        {
            var coins = Enumerable.Range(1, coinsPerPlayer).Select(x => new Coin()).ToArray();
            player.AcceptCoins(coins);
        }
    }

    public IGesjaaktReadOnlyGameState AsReadOnly()
    {
        return new GesjaaktReadOnlyGameState(this);
    }
}
