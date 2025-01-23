using Domain.Players;

namespace Domain.Game;

public class GameDealer : IGameDealer
{
    private IGameState _state;

    public GameDealer()
    {
        // TODO: DI
        this._state = new GameState();
    }

    public IGameStateReader State => this._state;

    public void AddPlayer(IPlayer player)
    {
        this._state.AddPlayer(player);
    }

    // TODO: moet deze functie er zijn? => Of moet dit niet onderdeel zijn van de constructor/init?
    public void RemoveCardsFromDeck(int amount)
    {
        _state.TakeCardsFromDeck(amount);
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

    public void Play()
    {
        while (!_state.Deck.IsEmpty())
        {
            this.NextPlayerPlays();
        }
    }

    // FIXME: this method should be private (?), how do we still unittest? => simple end states
    public void NextPlayerPlays()
    {
        var player = _state.PlayerOnTurn;
        var choice = player.Decide(_state);

        // TODO: REPLACE BY DESIGN PATTERN - STATE PATTERN?
        switch (choice)
        {
            case TurnAction.TAKECARD:
                player.AcceptCard(_state.Deck.DrawCard());
                player.AcceptCoins(_state.TakeCoins());
                break;

            case TurnAction.SKIPWITHCOIN:
                if (player.CoinsAmount > 0)
                {
                    var coin = player.GiveCoin();
                    _state.AddCoinToTable(coin);
                }
                else
                {
                    // FIXME: Dry from other case
                    player.AcceptCard(_state.Deck.DrawCard());
                    player.AcceptCoins(_state.TakeCoins());
                }

                break;
        }

        _state.NextPlayer();
    }

    public IPlayer Winner()
    {
        return _state.Players
            .ToDictionary(p => p, p => p.CardPoints() - p.CoinsAmount)
            .OrderByDescending(kvp => kvp.Value).First().Key;
    }
}