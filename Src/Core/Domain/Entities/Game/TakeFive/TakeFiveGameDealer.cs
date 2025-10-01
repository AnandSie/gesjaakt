using Domain.Interfaces.Games.TakeFive;
using System.Collections.Immutable;

namespace Domain.Entities.Game.TakeFive;

public class TakeFiveGameDealer : ITakeFiveGameDealer
{
    private readonly ITakeFiveGameState _gameState;

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
            var matchingRow = _gameState.CardRows
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

            // Regel 4
            // TODO: check if player.decide returns something which is allowable (not too larger).
            // If not within 0 - 3, throw error event and get first/random/least poitns row.
            // The least points does make the most sense..., this is what 99% people will implement

            var rowIndex = matchingRow?.RowIndex ?? RowChoiceToTake(player);
            if (DoesCardNotFitInRow(matchingRow) || IsRowFull(rowIndex))
            {
                PlayerTakesAllCardsFromRow(player, rowIndex);
            }
            _gameState.PlaceCard(card, rowIndex);
        }

    }
    private static bool DoesCardNotFitInRow(object? rowWhereCardCanBePlaced)
    {
        // TODO: add event
        return rowWhereCardCanBePlaced is null;
    }


    // Regel 3
    private bool IsRowFull(int rowIndex)
    {
        // TODO: add event
        return _gameState.CardRows.ElementAt(rowIndex).Count() == TakeFiveRules.MaxCardsInRowAllowed;
    }

    private int RowChoiceToTake(ITakeFivePlayer player)
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
