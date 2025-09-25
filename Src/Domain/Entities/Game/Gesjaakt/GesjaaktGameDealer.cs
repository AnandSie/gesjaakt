using Domain.Entities.Events;
using Domain.Interfaces.Games.Gesjaakt;

namespace Domain.Entities.Game.Gesjaakt;

public class GesjaaktGameDealer : IGesjaaktGameDealer
{
    private readonly IGesjaaktGameState _gameState;
    //private readonly ILogger<GesjaaktGameDealer> _logger;

    public event EventHandler<WarningEvent>? PlayerGesjaakt;
    public event EventHandler<InfoEvent>? SkippedWithCoin;
    public event EventHandler<InfoEvent>? CoinsDivided;

    public GesjaaktGameDealer(IGesjaaktGameState gameState)
    {
        //_logger = logger;
        // TODO: bevat deze dan ook een deck enzo..?
        _gameState = gameState;

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
            string message = $"!!!!!! GESJAAKT !!!!!! \n\t {player.Name} needs to take card {_gameState.OpenCardValue}";
            PlayerGesjaakt?.Invoke(this, new(message));
            //_logger.LogWarning(message);
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
                    string message = $"Amount of coins on table: {_gameState.AmountOfCoinsOnTable}";
                    SkippedWithCoin?.Invoke(this, new(message));
                    //_logger.LogInformation(message);
                    break;
            }
        }
    }

    public IOrderedEnumerable<IGesjaaktPlayer> GetPlayerResults()
    {
        return _gameState.Players.OrderBy(p => p.CardPoints() - p.CoinsAmount);
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
        _gameState.DivideCoins(coinsPerPlayer);
        string message = $"Every player gets {coinsPerPlayer} coins";
        CoinsDivided?.Invoke(this, new(message));
        //_logger.LogInformation(message);
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

    public void Add(IEnumerable<IGesjaaktPlayer> players)
    {
        foreach (var player in players)
        {
            _gameState.AddPlayer(player);
        }
    }
}