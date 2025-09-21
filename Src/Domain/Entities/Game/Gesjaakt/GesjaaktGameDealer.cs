using Domain.Interfaces;
using Domain.Interfaces.Games.Gesjaakt;

namespace Domain.Entities.Game.Gesjaakt;

public class GesjaaktGameDealer : IGameDealer<IGesjaaktReadOnlyPlayer>
{
    private readonly IGesjaaktGameState _gameState;
    private readonly ILogger<GesjaaktGameDealer> _logger;

    public GesjaaktGameDealer(IEnumerable<IGesjaaktPlayer> players, IGesjaaktGameState gameState, ILogger<GesjaaktGameDealer> logger)
    {
        _logger = logger;
        _gameState = gameState;

        foreach (var player in players)
        {
            _gameState.AddPlayer(player);
        }

        // TODO: Create a rules config object with we inject
        // TODO: Add ensures based on rules
        var amountToRemoveFromDeck = 9;
        _gameState.RemoveCardsFromDeck(amountToRemoveFromDeck);
    }

    public void Prepare()
    {
        DivideCoins();
        OpenFirstCardFromDeck();
    }

    public void Play()
    {
        while (!_gameState.Deck.IsEmpty() || _gameState.HasOpenCard)
        {
            PlayTurn();
            _gameState.NextPlayer();
        }
    }

    public IGesjaaktReadOnlyPlayer Winner()
    {
        return _gameState.Players
            .OrderBy(p => p.Points())
            .First()
            .AsReadOnly();
    }

    // TODO: make private
    public void PlayTurn()
    {
        // TODO: REPLACE BY DESIGN PATTERN - STATE PATTERN?
        var player = _gameState.PlayerOnTurn;

        if (player.CoinsAmount == 0)
        {
            _logger.LogWarning($"!!!!!! GESJAAKT !!!!!! \n\t {player.Name} needs to take card {_gameState.OpenCardValue}");
            HandleTakeCard(player);
        }
        else
        {
            // TODO: give the ReadOnly version
            switch (player.Decide(_gameState.AsReadOnly()))
            {
                case GesjaaktTurnOption.TAKECARD:
                    HandleTakeCard(player);
                    break;

                case GesjaaktTurnOption.SKIPWITHCOIN:
                    _gameState.AddCoinToTable(player.GiveCoin());
                    _logger.LogInformation($"Amount of coins on table: {_gameState.AmountOfCoinsOnTable}");
                    break;
            }
        }
    }

    public IOrderedEnumerable<IGesjaaktReadOnlyPlayer> GetPlayerResults()
    {
        return _gameState.AsReadOnly().Players.OrderBy(p => p.CardPoints() - p.CoinsAmount);
    }

    private void OpenFirstCardFromDeck()
    {
        _gameState.OpenNextCardFromDeck();
    }

    private void DivideCoins()
    {
        var coinsPerPlayer = _gameState.Players.Count() switch
        {
            3 or 4 or 5 => 11,
            6 => 9,
            7 => 7,
            _ => throw new Exception("The number of players is not equal to the rules for dividing coins expected"),
        };
        _logger.LogInformation($"Every player gets {coinsPerPlayer} coins");
        _gameState.DivideCoins(coinsPerPlayer);
    }

    private void HandleTakeCard(IGesjaaktPlayer player)
    {
        player.AcceptCard(_gameState.TakeOpenCard());
        player.AcceptCoins(_gameState.TakeCoins());

        if (!_gameState.Deck.IsEmpty())
        {
            _gameState.OpenNextCardFromDeck();
            PlayTurn(); // After taking card, you can play another turn
        }
    }
}