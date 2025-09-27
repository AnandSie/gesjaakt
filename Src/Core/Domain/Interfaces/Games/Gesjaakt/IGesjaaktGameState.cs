using Domain.Entities.Components;
using Domain.Entities.Events;
using Domain.Interfaces.Components;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.Gesjaakt;

public interface IGesjaaktGameState: IGameState<IGesjaaktPlayer>
{
    public IGesjaaktReadOnlyGameState AsReadOnly(); // TODO: Create a ToReadonlyGameState interface (similar to player)

    // GameWithRounds    
    public void NextPlayer();

    // ICardGame

    public void RemoveCardsFromDeck(int amount);
    public void OpenNextCardFromDeck();
    public ICard TakeOpenCard();
    
    // Gesjaakt specifiek
    public void DivideCoins(int coinsPerPlayer);
    public void AddCoinToTable(Coin coin);
    public IEnumerable<Coin> TakeCoinsFromTable();


    // IMPROVE - think, DRY with IGEsjaaktREadonlyGameState

    // ICardGame
    IGesjaaktPlayer PlayerOnTurn { get; }
    
    IReadOnlyDeck<Card> Deck { get; }
    
    bool HasOpenCard { get; }

    int OpenCardValue { get; }

    // Gesjaakt specifiek
    int AmountOfCoinsOnTable { get; }

    // Events
    event EventHandler<InfoEvent>? CardDrawnFromDeck;

}

