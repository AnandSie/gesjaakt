using Domain.Interfaces.Components;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.TakeFive;

public interface ITakeFiveMutableGameState: IMutableGameState<ITakeFivePlayerState>
{
    /// <summary> 
    /// Places a card in the specified row.
    /// Depending on the game rules, this may return a collection of cards gained as a result of the placement.
    /// The returned collection may be empty if no cards are gained. 
    /// </summary>
    /// 
    /// <param name="card">The card to place.</param>
    /// <param name="rowNumber">The row number to place the card in.</param>
    /// <returns>A collection of cards returned after placing the card, which may be empty.</returns>
    public IEnumerable<ICard> PlaceCard(ICard card, int rowNumber);

    /// <summary>
    /// Initializes the rows by drawing cards from the deck.
    /// This is called once during game setup to establish the starting layout.
    /// </summary>
    void InitializeRowsFromDeck();

}
