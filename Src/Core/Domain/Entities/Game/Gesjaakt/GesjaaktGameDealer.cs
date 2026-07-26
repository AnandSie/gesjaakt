using Domain.Entities.Events;
using Domain.Interfaces.Games.Gesjaakt;

namespace Domain.Entities.Game.Gesjaakt;

public class GesjaaktGameDealer : IGesjaaktGameDealer
{
    private readonly IGesjaaktGameState _gameState;

    public event EventHandler<SpecialEvent>? PlayerGesjaakt;
    public event EventHandler<OrdinaryEvent>? SkippedWithCoin;
    public event EventHandler<OrdinaryEvent>? CoinsDivided;
    public event EventHandler<FaultEvent>? PlayerDecideError;

    public GesjaaktGameDealer(IGesjaaktGameState gameState)
    {
        _gameState = gameState;

        // REFACTOR: Create a rules config object with we inject
        // REFACTOR: Add ensures based on rules
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

    public IOrderedEnumerable<IGesjaaktPlayer> GetPlayerResults()
    {
        return _gameState.Players.OrderBy(p => p.Points());
    }

    private void PlayTurn()
    {
        // REFACTOR: REPLACE BY DESIGN PATTERN - STATE PATTERN?
        var player = _gameState.PlayerOnTurn;

        if (player.CoinsAmount == 0)
        {
            string message = $"{player.Name} is GESJAAKT and has to take card {_gameState.OpenCardValue}";
            PlayerGesjaakt?.Invoke(this, new(nameof(PlayerGesjaakt), message, value: _gameState.OpenCardValue, actor: player.Name));
            HandleTakeCard(player);
        }
        else
        {
            switch (PlayerChoice(player))
            {
                case GesjaaktTurnOption.TAKECARD:
                    HandleTakeCard(player);
                    break;

                case GesjaaktTurnOption.SKIPWITHCOIN:
                    _gameState.AddCoinToTable(player.GiveCoin());
                    string message = $"{player.Name} skips with a coin. Amount of coins on table: {_gameState.AmountOfCoinsOnTable}";
                    SkippedWithCoin?.Invoke(this, new(nameof(SkippedWithCoin), message, value: _gameState.AmountOfCoinsOnTable, actor: player.Name));
                    break;
            }
        }
    }

    private GesjaaktTurnOption PlayerChoice(IGesjaaktPlayer player)
    {
        // REFACTOR - move try catch inside player. Player needs to return something sensible.., TakeFive also has this in player
        try
        {
            return player.Decide(_gameState.AsReadOnly());
        }
        catch (Exception e)
        {
            string message = $"player {player.Name} could not decide. So the he/she skips by playing a coin. The following error occured - {e.Message}";
            PlayerDecideError?.Invoke(this, new(nameof(PlayerDecideError), message, actor: player.Name));
            return GesjaaktTurnOption.SKIPWITHCOIN;
        }

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
        CoinsDivided?.Invoke(this, new(nameof(CoinsDivided), message, value: coinsPerPlayer));
    }

    private void HandleTakeCard(IGesjaaktPlayer player)
    {
        player.AcceptCard(_gameState.TakeOpenCard());
        player.AcceptCoins(_gameState.TakeCoinsFromTable());

        if (!_gameState.Deck.IsEmpty())
        {
            _gameState.OpenNextCardFromDeck();
            PlayTurn(); // Context: After taking card, you can play another turn
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