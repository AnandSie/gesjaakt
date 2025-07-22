using Domain.Interfaces.Components;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.TakeFive;

public interface ITakeFiveMutableGameState: IMutableGameState<ITakeFivePlayerState>
{
    /// <summary> 
    /// Places a card in the specified row.
    /// </summary>
    /// 
    /// <param name="card">The card to place.</param>
    /// <param name="rowNumber">The row number to place the card in.</param>
    public void PlaceCard(ICard card, int rowNumber);

    /// <summary> 
    /// Get Cards from a rownumber
    /// </summary>
    /// 
    /// <param name="rowNumber">The row number from which the cards will be retrived.</param>
    /// <returns> The collection of cards</returns> 
    public IEnumerable<ICard> GetCards(int rowNumber);

    /// <summary>
    /// Initializes the rows by drawing cards from the deck.
    /// This is called once during game setup to establish the starting layout.
    /// </summary>
    void InitializeRowsFromDeck();

}
