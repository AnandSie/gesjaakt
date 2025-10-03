using Domain.Entities.Events;
using Domain.Interfaces.Games.TakeFive;
using System.Collections.Immutable;

namespace Domain.Entities.Game.TakeFive;

public class TakeFiveGameDealer : ITakeFiveGameDealer
{
    private readonly ITakeFiveGameState _gameState;

    public event EventHandler<WarningEvent>? DiverCardIsPlayed;
    public event EventHandler<WarningEvent>? CardPlayedInFullRow;

    public TakeFiveGameDealer(ITakeFiveGameState gameState)
    {
        _gameState = gameState;
    }

    public IOrderedEnumerable<ITakeFivePlayer> GetPlayerResults()
    {
        return _gameState.Players
            .OrderBy(p => p.PenaltyCards.Sum(c => c.CowHeads));
    }

    public void Add(IEnumerable<ITakeFivePlayer> players)
    {
        foreach (var player in players)
        {
            _gameState.AddPlayer(player);
        }
    }

    public void Prepare()
    {
        _gameState.InitializeRowsFromDeck();
        _gameState.DealStartingCards(TakeFiveRules.StartNumberOfCards);
    }

    public void Play()
    {
        while (_gameState.Players.First().CardsCount > 0)
        {
            var cardsChoosenByPlayers = _gameState.Players
                .Select(p =>
                (
                    Player: p,
                    Card: p.Decide(_gameState.AsReadOnly()))
                )
                .OrderBy(pair => pair.Card.Value) // Regel 1
                .ToList();

            HandleChoosenCards(cardsChoosenByPlayers);
        }
    }

    private void HandleChoosenCards(IEnumerable<(ITakeFivePlayer player, TakeFiveCard card)> cardsFromPlayers)
    {
        foreach ((var player, var card) in cardsFromPlayers)
        {
            var rowWhereCardShouldBePlaced = _gameState.CardRows
                .Select((row, rowIndex) => new
                {
                    LastCardInRow = row.Last(),
                    RowIndex = rowIndex
                })
                // Regel 4
                .Where(pair => pair.LastCardInRow.Value < card.Value)
                // Regel 2
                .OrderBy(pair => card.Value - pair.LastCardInRow.Value)
                .FirstOrDefault();


            var rowIndex = rowWhereCardShouldBePlaced?.RowIndex ?? RowIndexThatPlayerChoose(player);
            if (IsCardLower(rowWhereCardShouldBePlaced, player, card) || IsRowFull(rowIndex, player, card))
            {
                PlayerTakesAllCardsFromRow(player, rowIndex);
            }
            _gameState.PlaceCard(card, rowIndex);
        }

    }
    private bool IsCardLower(object? rowWhereCardCanBePlaced, ITakeFivePlayer player, TakeFiveCard card)
    {
        bool result = rowWhereCardCanBePlaced is null;
        if (result)
        {
            var message = $"Diver Card - player {player.Name} played card {card.Value} that is lower than all rows.";
            DiverCardIsPlayed?.Invoke(this, new(message));
        }
        return result;
    }

    // Regel 3
    private bool IsRowFull(int rowIndex, ITakeFivePlayer player, TakeFiveCard card)
    {
        bool result = _gameState.CardRows.ElementAt(rowIndex).Count() == TakeFiveRules.MaxCardsInRowAllowed;
        if (result)
        {
            var message = $"TAKEFIVE - player {player.Name} played card {card.Value} in row that already is full.";
            CardPlayedInFullRow?.Invoke(this, new(message));
        }
        return result;
    }

    // Regel 4
    private int RowIndexThatPlayerChoose(ITakeFivePlayer player)
    {
        var immutableCardRows = _gameState.CardRows.Select(inner => inner.ToImmutableList()).ToImmutableList();
        return player.Decide(immutableCardRows);
    }

    private void PlayerTakesAllCardsFromRow(ITakeFivePlayer player, int rowNumber)
    {
        var cardsToTake = _gameState.GetCards(rowNumber);
        player.AcceptsPenaltyCards(cardsToTake);
    }
}
