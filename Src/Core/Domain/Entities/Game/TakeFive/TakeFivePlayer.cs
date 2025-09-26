using Domain.Interfaces.Games.TakeFive;

namespace Domain.Entities.Game.TakeFive;

public class TakeFivePlayer : ITakeFivePlayer
{
    private readonly ITakeFiveThinker _thinker;
    private readonly string _name;
    private readonly List<TakeFiveCard> _hand;
    private readonly List<TakeFiveCard> _penaltyCards;

    public string Name => _name;

    public TakeFivePlayer(ITakeFiveThinker thinker, string name)
    {
        _thinker = thinker;
        _name = name;
        _hand = new List<TakeFiveCard>();
        _penaltyCards = new List<TakeFiveCard>();
    }

    public int CardsCount => _hand.Count;

    public IReadOnlyCollection<TakeFiveCard> PenaltyCards => _penaltyCards.AsReadOnly();

    public void AccecptCards(IEnumerable<TakeFiveCard> cards)
    {
        _hand.AddRange(cards);
    }

    public void AccecptPenaltyCards(IEnumerable<TakeFiveCard> cards)
    {
        _penaltyCards.AddRange(cards);
    }

    public TakeFiveCard Decide(ITakeFiveReadOnlyGameState gameState)
    {
        // TODO: Implement/use thinker
        var cardValue = _thinker.Decide(gameState);
        return GetCard(cardValue);
    }

    // This method will be called when a player has played a card which does not fit and needs to choose a row to take
    public int Decide(IEnumerable<IEnumerable<TakeFiveCard>> cardRows)
    {
        return _thinker.Decide(cardRows);
    }

    private TakeFiveCard GetCard(int cardValue)
    {
        var card = _hand.FirstOrDefault(c => c.Value == cardValue);

        if (card == null)
        {
            // TODO: Use logger
            Console.WriteLine($"[WARN] Card with value {cardValue} not found. Falling back to first available card.");
            card = _hand.FirstOrDefault();
        }

        if (card == null)
        { 
            throw new InvalidOperationException("No cards left in hand.");
        }

        _hand.Remove(card);
        return card;
    }

    public ITakeFiveReadOnlyPlayer AsReadOnly()
    {
        return new TakeFiveReadOnlyPlayer(this);
    }

}
