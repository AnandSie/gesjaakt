using Domain.Interfaces.Games.TakeFive;
using System.Collections.Immutable;

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

    // NOTE: This is the main method in the game where the player decides which card to play
    public TakeFiveCard Decide(ITakeFiveReadOnlyGameState gameState)
    {
        _thinker.SetState(_hand.ToImmutableList());
        var cardValue = _thinker.Decide(gameState);
        return GetCard(cardValue);
    }

    // NOTE: This method will be called when a player has played a card which does not fit and needs to choose a row to take
    public int Decide(IEnumerable<IEnumerable<TakeFiveCard>> cardRows)
    {
        //Console.WriteLine(); // TODO: maak bericht en later event
        return _thinker.Decide(cardRows);
    }

    private TakeFiveCard GetCard(int cardValue)
    {
        var card = _hand.FirstOrDefault(c => c.Value == cardValue);

        // TODO (WARNING LEVEL) - Consider using events to communicate this situation 
        // string message = $"[WARN] Card with value {cardValue} not found. Falling back to first available card.";
        card ??= _hand.FirstOrDefault();

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

    public override string ToString()
    {
        // REFACTOR - use extension method for the IEnumerable<TakeFiveCard>
        return $"{_name ?? "unkown"} with {Points()} points - Cards {string.Join(", ", this.PenaltyCards)}";
    }

    private int Points()
    {
        return this.PenaltyCards.Sum(pc => pc.CowHeads);
    }
}
