﻿using Domain.Entities.Cards;
using Domain.Interfaces;
using System.Text;

namespace Domain.Entities.Game;

public class GameState : IGameState
{
    private readonly IList<IPlayer> _players;
    private readonly ILogger<GameState> _logger;
    private int _playerIndex;
    private ICollection<ICoin> _coinsOnTable;
    private IDeck _deck;
    private ICard? _openCard;

    public GameState(IEnumerable<IPlayer> players, ILogger<GameState> logger)
    {
        _players = players.ToList();
        _playerIndex = 0;
        _coinsOnTable = new HashSet<ICoin>();
        _deck = new Deck(3, 35); // TODO: extract to config
        _logger = logger;
    }

    IEnumerable<IPlayerActions> IGameStateWriter.Players
    {
        get
        {
            return _players;
        }
    }

    IEnumerable<IPlayerState> IGameStateReader.Players
    {
        get
        {
            return _players;
        }
    }

    public IEnumerable<IPlayer> Players => _players;

    public void OpenNextCardFromDeck()
    {
        _openCard = _deck.DrawCard();
        _logger.LogInformation($"Card drawn: {_openCard.Value}. Cards left: {Deck.AmountOfCardsLeft()}");
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

    public bool HasOpenCard => _openCard != null;

    public IDeckState Deck => _deck;

    public int AmountOfCoinsOnTable => _coinsOnTable.Count();

    public IPlayerState PlayerOnTurn => _players[_playerIndex];


    public void AddCoinToTable(ICoin coin)
    {
        _coinsOnTable.Add(coin);
    }

    public void AddPlayer(IPlayerActions newPlayer)
    {
        // FIXME: casting is suboptimal.., code smell?
        _players.Add((IPlayer)newPlayer);
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
}
