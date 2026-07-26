using Domain.Entities.Events;
using Domain.Interfaces.Components;
using Domain.Interfaces.Games.BaseGame;
using Domain.Interfaces.Games.TakeFive;

namespace Domain.Entities.Game.TakeFive;

public class TakeFiveGameState : ITakeFiveGameState
{
    private readonly IMutableDeck<TakeFiveCard> _deck;
    private readonly HashSet<ITakeFivePlayer> _players;
    private readonly List<List<TakeFiveCard>> _cardRows;
    private bool _isInitialized = false;

    public event EventHandler<OrdinaryEvent>? CardIsPlaced;
    public event EventHandler<NotableEvent>? RowIsTaken;

    public TakeFiveGameState(IDeckFactory<TakeFiveCard> deckFactory)
    {
        _deck = deckFactory.Create();
        _players = new HashSet<ITakeFivePlayer>();

        _cardRows = Enumerable.Range(0, TakeFiveRules.NumberOfRows)
            .Select(_ => new List<TakeFiveCard>())
            .ToList();
    }

    public ITakeFiveReadOnlyGameState AsReadOnly() => new TakeFiveReadOnlyGameState(this);

    public IMutableDeck<TakeFiveCard> Deck => _deck;

    public IEnumerable<IEnumerable<TakeFiveCard>> CardRows => _cardRows;

    public IEnumerable<ITakeFivePlayer> Players => _players;

    public void AddPlayer(ITakeFivePlayer player)
    {
        _players.Add(player);
    }

    public void InitializeRowsFromDeck()
    {
        if (_isInitialized) return;

        for (int i = 0; i < TakeFiveRules.NumberOfRows; i++)
        {
            var card = _deck.DrawCard();
            _cardRows.ElementAt(i).Add(card);
        }
        _isInitialized = true;
    }

    public void PlaceCard(TakeFiveCard card, int rowNumber)
    {
        _cardRows.ElementAt(rowNumber).Add(card);
        this.CardIsPlaced?.Invoke(this, new(nameof(CardIsPlaced), $"card with value {card.Value} is placed in row {rowNumber + 1}", value: card.Value));
    }

    public IEnumerable<TakeFiveCard> GetCards(int rowNumber)
    {
        var cardRow = _cardRows.ElementAt(rowNumber);
        var result = cardRow.ToHashSet();

        cardRow.Clear();

        // The cow heads are what taking a row actually costs, so they ride along
        // as the event's value and become a mean in the run summary.
        var cowHeads = result.Sum(c => c.CowHeads);
        this.RowIsTaken?.Invoke(this, new(nameof(RowIsTaken), $"Cards of row {rowNumber + 1} are taken ({cowHeads} cow heads)", value: cowHeads));
        return result;
    }

    public void DealStartingCards(int cardsPerPlayer)
    {
        foreach (var player in _players)
        {
            var cards = DrawCards(cardsPerPlayer);
            player.AccecptCards(cards);
        }
    }

    public override string ToString()
    {
        throw new NotImplementedException();
    }

    private List<TakeFiveCard> DrawCards(int count) =>
            Enumerable.Range(0, count)
              .Select(_ => _deck.DrawCard())
              .ToList();
}
