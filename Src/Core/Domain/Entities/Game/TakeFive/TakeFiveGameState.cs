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

    public event EventHandler<InfoEvent>? CardIsPlaced;
    public event EventHandler<InfoEvent>? RowIsTaken;

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
        this.CardIsPlaced?.Invoke(this, new($"card with value {card.Value} is placed in row {rowNumber + 1}"));
    }

    public IEnumerable<TakeFiveCard> GetCards(int rowNumber)
    {
        var cardRow = _cardRows.ElementAt(rowNumber);
        var result = cardRow.ToHashSet();

        cardRow.Clear();

        this.RowIsTaken?.Invoke(this, new($"Cards of row {rowNumber + 1} are taken"));
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
