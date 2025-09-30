using Domain.Entities.Events;
using Domain.Interfaces.Games.TakeFive;

namespace Domain.Entities.Game.TakeFive;

// IMPROVE: Try to make generic such that we have one gameDealer
// e.g. The GameSTate does not have to have two seperate methods (e.g. InitializeRowsFromDeck/DealStartingCards => prepare)
// => a lot of thing will go to the GameSTate (which is fine (?)). It will probably also be easier if we create rule objeect

// Play {
// While(!GameRules.GameShouldEnd(gameState)
// { GameRules.PlayRound(GameState) } 

// Prepare {GameRules.Prepare(GameState)

// Winner {GameRules.Winner(GameStat)

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
                    // TODO: Create a try catch + event if error
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

            // IMPROVE - Create Event for Decide2 (for statistics during game)

            // Regel 4

            // TODO: Give Immutable cards (nested and inner nested)
            // TODO: check if player.decide returns something which is allowable (not too larger). If not within 0 - 3, throw error event and get first/random/least poitns row. The least points does make the most sense..., this is what 99% people will implement
            var rowIndex = matchingRow?.RowIndex ?? player.Decide(_gameState.CardRows);

            // Regel 3
            if (_gameState.CardRows.ElementAt(rowIndex).Count() == TakeFiveRules.MaxCardsInRowAllowed || matchingRow == null)
            {
                PlayerTakesCardFromRow(player, rowIndex);
            }

            _gameState.PlaceCard(card, rowIndex);
        }
    }

    private void PlayerTakesCardFromRow(ITakeFivePlayer player, int rowNumber)
    {
        var cardsToTake = _gameState.GetCards(rowNumber);
        player.AccecptPenaltyCards(cardsToTake);
    }
}
