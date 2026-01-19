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

    public event EventHandler<ErrorEvent>? DecideError;
    public event EventHandler<ErrorEvent>? CardNotFound;

    public string Name => _name;

    public TakeFivePlayer(ITakeFiveThinker thinker)
    {
        _thinker = thinker;
        _name = thinker.Name;
        _hand = new List<TakeFiveCard>();
        _penaltyCards = new List<TakeFiveCard>();
    }

    public int CardsCount => _hand.Count;

    public IReadOnlyCollection<TakeFiveCard> PenaltyCards => _penaltyCards.AsReadOnly();

    public void AccecptCards(IEnumerable<TakeFiveCard> cards)
    {
        _hand.AddRange(cards);
    }

    public void AcceptsPenaltyCards(IEnumerable<TakeFiveCard> cards)
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
            cardValue = _thinker.Decide(gameState).Value;
        }
        catch (Exception e)
        {
            string message = $"Decide Exception - Player {Name} could not decide. So a random card is played. Error message: {e.Message} ";
            DecideError?.Invoke(this, new(message));

            cardValue = _hand.First().Value;
        }

        return GetCard(cardValue);
    }

    // NOTE: This method will be called when a player has played a card which does not fit and needs to choose a row to take
    public int Decide(ImmutableList<ImmutableList<TakeFiveCard>> cardRows)
    {
        var backupChoice = 0;
        int result;
        try
        {
            result = _thinker.Decide(cardRows);

            int minAllowed = 0;
            int maxAllowed = TakeFiveRules.NumberOfRows - 1; // zero index
            if (result < minAllowed || result > maxAllowed)
            {
                string incorrectValueMessage = $"Decide Incorrect Result - Player {this.Name} choice for row to take of {result} is not between {minAllowed} and {maxAllowed}. So a random row is taken";
                DecideError?.Invoke(this, new(incorrectValueMessage));
                result = backupChoice;
            }
        }
        catch (Exception e)
        {
            string excceptionMessage = $"Decide Exception - Player {this.Name} could not decide. So a random row is taken. Error message: {e.Message} ";
            DecideError?.Invoke(this, new(excceptionMessage));

            result = backupChoice;
        }

        return result;
    }


    private TakeFiveCard GetCard(int cardValue)
    {
        if (_hand.Count == 0)
        {
            throw new InvalidOperationException("No cards left in hand.");
        }

        var card = _hand.FirstOrDefault(c => c.Value == cardValue);
        if (card == null)
        {
            string message = $"Card with value {cardValue} not found. Falling back to first available card.";
            CardNotFound?.Invoke(this, new(message));
            card = _hand.First();
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
        return $"{_name ?? "unkown"} with {PenaltyPoints()} points - Cards {string.Join(", ", this.PenaltyCards)}";
    }

    private int PenaltyPoints()
    {
        return PenaltyCards.Sum(pc => pc.CowHeads);
    }
}
