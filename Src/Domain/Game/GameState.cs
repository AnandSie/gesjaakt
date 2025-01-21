﻿using Domain.Cards;
using Domain.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Game;

public class GameState : IGameState
{
    private IList<IPlayer> _players;
    private int _playerIndex;
    private ICollection<ICoin> _coinsOnTable;
    private IDeck _deck;

    public GameState()
    {
        _players = new List<IPlayer>();
        _playerIndex = 0;
        _coinsOnTable = new HashSet<ICoin>();
    }

    public IEnumerable<IPlayer> Players => _players;

    public ICard OpenCard => throw new NotImplementedException();

    public IDeck Deck => _deck;

    public int AmountOfCoinsOnTable => _coinsOnTable.Count();

    public IPlayer PlayerOnTurn => _players[_playerIndex];

    public void AddCoinToTable(ICoin coin)
    {
        _coinsOnTable.Add(coin);
    }

    public void AddPlayer(IPlayer player)
    {
        this._players.Add(player);
    }

    public void NextPlayer()
    {
        if (_playerIndex == (_players.Count - 1))
        {
            _playerIndex = 0;
        }
        else
        {
            _playerIndex++;
        }
    }
}
