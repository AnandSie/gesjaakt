using Domain.Entities.Players;
using Domain.Interfaces;

namespace Domain.Entities.Game;

public class GameDealer : IGameDealer
{
    private readonly IGameState _state;

    public GameDealer(IEnumerable<IPlayer> players)
    {
        // TODO: DI
        _state = new GameState();
        foreach (var player in players)
        {
            _state.AddPlayer(player);
        }

        // TODO: Create a rules config object with we inject
        // TODO: Add ensures based on rules
        var amountToRemoveFromDeck = 9;
        _state.RemoveCardsFromDeck(amountToRemoveFromDeck);
    }

    public IGameStateReader State => _state;

    private void DivideCoins()
    {
        // FIXME: COntinously casting is suboptimal
        var coinsPerPlayer = ((IGameStateReader)_state).Players.Count() switch
        {
            3 or 4 or 5 => 11,
            6 => 9,
            7 => 7,
            _ => throw new Exception("The amoutn of players is not equal to the rules for dividing coins expected"),
        };
        Console.WriteLine($"Every player gets {coinsPerPlayer} coins");
        foreach (var player in ((IGameStateWriter)_state).Players)
        {
            var coins = Enumerable.Range(1, coinsPerPlayer).Select(x => new Coin()).ToArray();
            player.AcceptCoins(coins);
        }
    }

    public void Play()
    {
        DivideCoins();
        PlayFirstCard();
        while (!_state.Deck.IsEmpty())
        {
            PlayTurn();
        }
    }

    public IPlayerState Winner()
    {
        return ((IGameStateReader)_state).Players
            .OrderBy(p => p.CardPoints() - p.CoinsAmount)
            .First();
    }

    public void PlayFirstCard()
    {
        _state.OpenNextCardFromDeck();
    }

    public void PlayTurn()
    {
        // TODO: REPLACE BY DESIGN PATTERN - STATE PATTERN?
        var player = (IPlayer)_state.PlayerOnTurn;

        if (player.CoinsAmount == 0)
        {
            Console.WriteLine("!!!!!! GESJAAKT !!!!!! ");
            Console.WriteLine("You need to take a card");
            HandleTakeCard(player);
        }
        else
        {
            switch (player.Decide(_state))
            {
                case TurnAction.TAKECARD:
                    HandleTakeCard(player);
                    break;

                case TurnAction.SKIPWITHCOIN:
                    _state.AddCoinToTable(player.GiveCoin());
                    break;
            }
        }

        _state.NextPlayer();
    }

    private void HandleTakeCard(IPlayerActions player)
    {
        player.AcceptCard(_state.TakeOpenCard());
        player.AcceptCoins(_state.TakeCoins());

        if (!_state.Deck.IsEmpty())
        {
            _state.OpenNextCardFromDeck();
            PlayTurn();
        }
    }
}