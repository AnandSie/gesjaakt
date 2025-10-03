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
    public abstract TakeFiveCard Decide(ITakeFiveReadOnlyGameState gameState);

    /// <summary>
    /// Determines which card row the thinker wants to take based on the current rows of cards.
    /// </summary>
    /// <param name="gameState">
    /// The current state of the game, represented as a collection of card rows. 
    /// Each row is a collection of <see cref="TakeFiveCard"/> objects.
    /// </param>
    /// <returns>
    /// The index of the card row the thinker chooses to take. 
    /// <b>Important:</b> This index is zero-based.
    /// </returns>
    /// <remarks>
    /// Typically, the thinker will choose the row with the least penalty points (cow heads),
    /// but more advanced strategies may take other considerations into account.
    /// </remarks>
    public abstract int Decide(IEnumerable<IEnumerable<TakeFiveCard>> gameState);

    public void SetState(IImmutableList<TakeFiveCard> state)
    {
        _hand = state;
    }
}
