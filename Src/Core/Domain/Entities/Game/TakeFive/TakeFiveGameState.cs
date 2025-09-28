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
    }

    public IEnumerable<TakeFiveCard> GetCards(int rowNumber)
    {
        var cardRow = _cardRows.ElementAt(rowNumber);
        var result = cardRow.ToHashSet();

        cardRow.Clear();
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

    private List<TakeFiveCard> DrawCards(int count) =>
            Enumerable.Range(0, count)
              .Select(_ => _deck.DrawCard())
              .ToList();
}
