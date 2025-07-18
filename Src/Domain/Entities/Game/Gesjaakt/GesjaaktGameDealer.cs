using Domain.Entities.Components;
using Domain.Entities.Players;
using Domain.Interfaces;

namespace Domain.Entities.Game.Gesjaakt;

public class GesjaaktGameDealer : IGameDealer
{
    private readonly IGameState _state;
    private readonly ILogger<GesjaaktGameDealer> _logger;

    public GesjaaktGameDealer(IEnumerable<IPlayer> players, IGameState gameState, ILogger<GesjaaktGameDealer> logger)
    {
        _logger = logger;
        _state = gameState;

        foreach (var player in players)
        {
            _state.AddPlayer(player);
        }

        // TODO: Create a rules config object with we inject
        // TODO: Add ensures based on rules
        var amountToRemoveFromDeck = 9;
        _state.RemoveCardsFromDeck(amountToRemoveFromDeck);
    }

    public void Prepare()
    {
        DivideCoins();
        OpenFirstCardFromDeck();
    }

    public void Play()
    {
        while (!_state.Deck.IsEmpty() || _state.HasOpenCard)
        {
            PlayTurn();
            NextPlayer();
        }
    }

    public IPlayerState Winner()
    {
        return ((IGameStateReader)_state).Players
            .OrderBy(p => p.Points())
            .First();
    }


    public void NextPlayer()
    {
        _state.NextPlayer();
    }

    public void PlayTurn()
    {
        // TODO: REPLACE BY DESIGN PATTERN - STATE PATTERN?
        var player = (IPlayer)_state.PlayerOnTurn;

        if (player.CoinsAmount == 0)
        {
            _logger.LogWarning($"!!!!!! GESJAAKT !!!!!! \n\t {player.Name} needs to take card {_state.OpenCardValue}");
            HandleTakeCard(player);
        }
        else
        {
            switch (player.Decide(_state))
            {
                case GesjaaktTurnOption.TAKECARD:
                    HandleTakeCard(player);
                    break;

                case GesjaaktTurnOption.SKIPWITHCOIN:
                    _state.AddCoinToTable(player.GiveCoin());
                    _logger.LogInformation($"Amount of coins on table: {_state.AmountOfCoinsOnTable}");
                    break;
            }
        }
    }

    public IEnumerable<IPlayer> GameResultOrdended()
    {
        return _state.Players.OrderBy(p => p.CardPoints() - p.CoinsAmount);
    }

    private void OpenFirstCardFromDeck()
    {
        _state.OpenNextCardFromDeck();
    }

    private void DivideCoins()
    {
        // FIXME: COntinously casting is suboptimal
        var coinsPerPlayer = ((IGameStateReader)_state).Players.Count() switch
        {
            3 or 4 or 5 => 11,
            6 => 9,
            7 => 7,
            _ => throw new Exception("The number of players is not equal to the rules for dividing coins expected"),
        };
        _logger.LogInformation($"Every player gets {coinsPerPlayer} coins");
        foreach (var player in ((IGameStateWriter)_state).Players)
        {
            var coins = Enumerable.Range(1, coinsPerPlayer).Select(x => new Coin()).ToArray();
            player.AcceptCoins(coins);
        }
    }

    private void HandleTakeCard(IGesjaaktActions player)
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