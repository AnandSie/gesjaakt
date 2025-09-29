using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.TakeFive;
using System.Collections.Immutable;

namespace Application.TakeFive.Thinkers;

public abstract class BaseTakeFiveThinker : ITakeFiveThinker
{
    protected IImmutableList<TakeFiveCard> _hand = [];

    /// <summary>
    /// Determines which card the thinker wants to play based on the current game state.
    /// </summary>
    /// <param name="gameState">The read-only snapshot of the current TakeFive game state.</param>
    /// <returns>
    /// The value of the card the thinker chooses to play. 
    /// <b>Important:</b> The returned <c>int</c> represents the card's value, 
    /// not its position in the player's hand.
    /// </returns>
    public abstract int Decide(ITakeFiveReadOnlyGameState gameState);

    public abstract int Decide(IEnumerable<IEnumerable<TakeFiveCard>> gameState);

    public void SetState(IImmutableList<TakeFiveCard> state)
    {
        _hand = state;
    }
}
