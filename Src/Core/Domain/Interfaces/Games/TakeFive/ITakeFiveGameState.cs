using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.TakeFive;

public interface ITakeFiveGameState: IGameState<ITakeFivePlayer>, IToReadOnly<ITakeFiveReadOnlyGameState>
{
    /// <summary> 
    /// Places a card in the specified row.
    /// </summary>
    /// 
    /// <param name="card">The card to place.</param>
    /// <param name="rowNumber">The row number to place the card in.</param>
    public void PlaceCard(TakeFiveCard card, int rowNumber);

    /// <summary> 
    /// Get Cards from a rownumber on the table
    /// </summary>
    /// 
    /// <param name="rowNumber">The row number from which the cards will be retrived.</param>
    /// <returns> The collection of cards</returns> 
    public IEnumerable<TakeFiveCard> GetCards(int rowNumber);

    /// <summary>
    /// Initializes the rows by drawing cards from the deck.
    /// This is called once during game setup to establish the starting layout.
    /// </summary>
    void InitializeRowsFromDeck();

    /// <summary>
    /// Distributes the given number of cards from the deck to each player during game setup.
    /// </summary>
    /// <param name="cardsPerPlayer">The number of cards to deal to each player.</param>
    void DealStartingCards(int cardsPerPlayer);

    IEnumerable<IEnumerable<TakeFiveCard>> CardRows { get; }

}
