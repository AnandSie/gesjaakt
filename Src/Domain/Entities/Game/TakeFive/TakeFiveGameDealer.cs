using Domain.Interfaces;
using Domain.Interfaces.Games.TakeFive;

namespace Domain.Entities.Game.TakeFive;

// TODO: Try to make generic such that we have one gameDealer
// e.g. The GameSTate does not have to have two seperate methods (e.g. InitializeRowsFromDeck/DealStartingCards => prepare)
// => a lot of thing will go to the GameSTate (which is fine (?)). It will probably also be easier if we create rule objeect

// Play {
// While(!GameRules.GameShouldEnd(gameState)
// { GameRules.PlayRound(GameState) } 

// Prepare {GameRules.Prepare(GameState)

// Winner {GameRules.Winner(GameStat)

public class TakeFiveGameDealer : IGameDealer<ITakeFiveReadOnlyPlayer>
{
    private readonly ITakeFiveGameState _gameState;

    public TakeFiveGameDealer(ITakeFiveGameState gameState)
    {
        _gameState = gameState;
    }

    public IOrderedEnumerable<ITakeFiveReadOnlyPlayer> GetPlayerResults()
    {
        throw new NotImplementedException();
    }

    public void Prepare()
    {
        _gameState.InitializeRowsFromDeck();
        var startAmount = 10; // TODO: Config/Object Rule
        _gameState.DealStartingCards(startAmount);
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
            var rowIndex = matchingRow?.RowIndex ?? player.Decide(_gameState.CardRows);

            // Regel 3
            if (_gameState.CardRows.ElementAt(rowIndex).Count() == 5 || matchingRow == null)
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

    public ITakeFiveReadOnlyPlayer Winner()
    {
        ITakeFivePlayer winner = _gameState.Players
            .OrderBy(p => p.PenaltyCards.Sum(c => c.CowHeads))
            .First();

        return winner.AsReadOnly();
    }
}
