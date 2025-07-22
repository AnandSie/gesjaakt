using Domain.Entities.Components;
using Domain.Interfaces.Components;
using Domain.Interfaces.Games.BaseGame;
using Domain.Interfaces.Games.TakeFive;

namespace Domain.Entities.Game.TakeFive;

public class TakeFiveGameState : ITakeFiveMutableGameState
{
    private readonly Deck _deck;
    private readonly HashSet<ITakeFivePlayerState> _players;
    private readonly List<List<ICard>> _cardRows;
    private bool _isInitialized = false;

    public TakeFiveGameState()
    {
        _deck = new Deck(1, 104); // TODO: place this into config/rule object
        _players = new HashSet<ITakeFivePlayerState>();

        var rowsToCreate = 4;// TODO: place into config/rule object
        _cardRows = Enumerable.Range(0, rowsToCreate)
            .Select(_ => new List<ICard>())
            .ToList();
    }

    public ITakeFiveReadOnlyGameState AsReadOnly() => new TakeFiveReadOnlyGameState(this);
    public Deck Deck => _deck;

    public IEnumerable<ITakeFivePlayerState> Players => _players;

    public IEnumerable<IEnumerable<ICard>> CardRows => _cardRows;

    // FIXME: can't we just inject players in constructor?
    public void AddPlayer(ITakeFivePlayerState player)
    {
        _players.Add(player);
    }

    public void InitializeRowsFromDeck()
    {
        if (_isInitialized) return;

        var rowsToInitalize = 4;

        for (int i = 0; i < rowsToInitalize; i++)
        {
            var card = _deck.DrawCard();
            _cardRows.ElementAt(i).Add(card);
        }
        this._isInitialized = true;
    }

    public void PlaceCard(ICard card, int rowNumber)
    {
        _cardRows.ElementAt(rowNumber).Add(card);
    }

    public IEnumerable<ICard> GetCards(int rowNumber)
    {
        List<ICard> cardRow = _cardRows.ElementAt(rowNumber);
        var result = cardRow.ToHashSet();

        cardRow.Clear();
        return result;
    }
}
