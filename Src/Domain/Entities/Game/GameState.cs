using Domain.Entities.Cards;
using Domain.Interfaces;
using System.Text;

namespace Domain.Entities.Game;

public class GameState : IGameState
{
    private IList<IPlayer> _players;
    private int _playerIndex;
    private ICollection<ICoin> _coinsOnTable;
    private IDeck _deck;
    private ICard? _openCard;

    public GameState()
    {
        _players = new List<IPlayer>();
        _playerIndex = 0;
        _coinsOnTable = new HashSet<ICoin>();
        // TODO: extract with DI
        _deck = new Deck(3, 35); // TODO: extract to config
    }

    public IEnumerable<IPlayer> Players => _players;

    public void OpenNextCardFromDeck()
    {
        _openCard = _deck.DrawCard();
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

    // TODO: Custom Exception
    public int OpenCardValue => _openCard?.Value ?? throw new Exception("There is no card yet");

    public IDeckState Deck => _deck;

    public int AmountOfCoinsOnTable => _coinsOnTable.Count();

    public IPlayer PlayerOnTurn => _players[_playerIndex];

    public void AddCoinToTable(ICoin coin)
    {
        _coinsOnTable.Add(coin);
    }

    public void AddPlayer(IPlayer player)
    {
        _players.Add(player);
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

        sb.AppendLine("Player states");
        foreach (var player in _players)
        {
            sb.AppendLine($"- {player.ToString()}");
        }
        sb.AppendLine($"Card open: {OpenCardValue}");
        sb.AppendLine($"Coins On Table {AmountOfCoinsOnTable}");
        sb.AppendLine($"Cards Left: {Deck.AmountOfCardsLeft()}");

        return sb.ToString();
    }
}
