using Domain.Interfaces;
using Domain.Interfaces.Games.Gesjaakt;

namespace Domain.Entities.Game.Gesjaakt;

public class GesjaaktGameDealer : IGameDealer<IGesjaaktPlayerState>
{
    private readonly IGesjaaktGameState _state;
    private readonly ILogger<GesjaaktGameDealer> _logger;

    public GesjaaktGameDealer(IEnumerable<IGesjaaktPlayer> players, IGesjaaktGameState gameState, ILogger<GesjaaktGameDealer> logger)
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
            _state.NextPlayer();
        }
    }

    public IGesjaaktPlayerState Winner()
    {
        return _state.Players
            .OrderBy(p => p.Points())
            .First();
    }

    public void PlayTurn()
    {
        // TODO: REPLACE BY DESIGN PATTERN - STATE PATTERN?
        var player = (IGesjaaktPlayer)_state.PlayerOnTurn;

        if (player.CoinsAmount == 0)
        {
            _logger.LogWarning($"!!!!!! GESJAAKT !!!!!! \n\t {player.Name} needs to take card {_state.OpenCardValue}");
            HandleTakeCard(player);
        }
        else
        {
            // TODO: give the ReadOnly version
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

    public IOrderedEnumerable<IGesjaaktPlayerState> GetPlayerResults()
    {
        // FIXME: De IGameState interface zou gewoon via één plek PLayers moeten hebben, niet en via reader en via writer (dit zou methods moeten zijn)
        return ((IGesjaaktReadOnlyGameState)_state).Players.OrderBy(p => p.CardPoints() - p.CoinsAmount);
    }

    private void OpenFirstCardFromDeck()
    {
        _state.OpenNextCardFromDeck();
    }

    private void DivideCoins()
    {
        // FIXME: COntinously casting is suboptimal
        var coinsPerPlayer = ((IGesjaaktReadOnlyGameState)_state).Players.Count() switch
        {
            3 or 4 or 5 => 11,
            6 => 9,
            7 => 7,
            _ => throw new Exception("The number of players is not equal to the rules for dividing coins expected"),
        };
        _logger.LogInformation($"Every player gets {coinsPerPlayer} coins");
        _state.DivideCoins(coinsPerPlayer);
    }

    private void HandleTakeCard(IGesjaaktPlayerActions player)
    {
        player.AcceptCard(_state.TakeOpenCard());
        player.AcceptCoins(_state.TakeCoins());

        if (!_state.Deck.IsEmpty())
        {
            _state.OpenNextCardFromDeck();
            PlayTurn(); // After taking card, you can play another turn
        }
    }
}