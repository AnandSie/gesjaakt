﻿using Domain.Players;

namespace Domain.Game;

public class GameDealer : IGameDealer
{
    private IGameState _state;

    public GameDealer()
    {
        this._state = new GameState();
    }

    public IGameStateReader State => this._state;

    public void AddPlayer(IPlayer player)
    {
        this._state.AddPlayer(player);
    }

    public void DivideCoins(int coinsAmount)
    {
        var coinsPerPlayer = (int)Math.Floor((decimal)coinsAmount / _state.Players.Count());
        foreach (var player in this._state.Players)
        {
            var coins = Enumerable.Range(1, coinsPerPlayer).Select(x => new Coin()).ToArray();
            player.AcceptCoins(coins);
        }
    }

    public void NextPlayerPlays()
    {
        var player = _state.PlayerOnTurn;
        var choice = player.Decide(_state);

        // TODO: REPLACE BY DESIGN PATTERN - STATE PATTERN?
        switch (choice)
        {
            case TurnAction.TAKECARD:
                var card = _state.Deck.DrawCard();
                player.AcceptCard(card);
                break;
            case TurnAction.SKIPWITHCOIN:
                var coin = player.GiveCoin();
                _state.AddCoinToTable(coin);
                break;
        }

        _state.NextPlayer();
    }

    public void CalculateEndScore()
    {
        throw new NotImplementedException();
    }
}