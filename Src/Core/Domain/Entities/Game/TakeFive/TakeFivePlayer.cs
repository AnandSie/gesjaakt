using Domain.Entities.Events;
using Domain.Interfaces.Games.TakeFive;
using System.Collections.Immutable;

namespace Domain.Entities.Game.TakeFive;

public class TakeFivePlayer : ITakeFivePlayer
{
    private readonly ITakeFiveThinker _thinker;
    private readonly string _name;
    private readonly List<TakeFiveCard> _hand;
    private readonly List<TakeFiveCard> _penaltyCards;

    // TODO: attach to collector
    public event EventHandler<ErrorEvent>? DecideError;

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
        int cardValue;
        try
        {
            cardValue = _thinker.Decide(gameState);
        }
        catch (Exception e)
        {
            string message = $"Decide Exception - Player {this.Name} could not decide. So a random card is played. Error message: {e.Message} ";
            DecideError?.Invoke(this, new(message));

            cardValue = this._hand.First().Value;
        }

        return GetCard(cardValue);
    }

    // NOTE: This method will be called when a player has played a card which does not fit and needs to choose a row to take
    public int Decide(IEnumerable<IEnumerable<TakeFiveCard>> cardRows)
    {
        int result;
        try
        {
            result = _thinker.Decide(cardRows);
        }
        catch (Exception e)
        {
            string message = $"Decide Exception - Player {this.Name} could not decide. So a random row is taken. Error message: {e.Message} ";
            DecideError?.Invoke(this, new(message));

            result = 0;

        }
        return result;
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
        return PenaltyCards.Sum(pc => pc.CowHeads);
    }
}
