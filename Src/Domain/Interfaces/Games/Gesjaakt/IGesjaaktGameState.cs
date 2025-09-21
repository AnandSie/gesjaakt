using Domain.Entities.Components;
using Domain.Interfaces.Components;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.Gesjaakt;

// TODO: SPlit in gesjaakt en ordinary
public interface IGesjaaktGameState: IGameState<IGesjaaktPlayer>
{
    // TODO: Create a ToReadonlyGameState interface (similar to player)
    public IGesjaaktReadOnlyGameState AsReadOnly();

    // GameWithRounds    
    public void NextPlayer();

    // ICardGame

    public void RemoveCardsFromDeck(int amount);
    public void OpenNextCardFromDeck();
    public ICard TakeOpenCard(); // FIXME: De GameState moet eigenlijk de kaart aan speler geven
    
    // Gesjaakt specifiek
    public void DivideCoins(int coinsPerPlayer);
    public void AddCoinToTable(ICoin coin);
    public IEnumerable<ICoin> TakeCoins(); // FIXME: De gamestate moet dit aan geven aan spelers


    // TODO - think, DRY with IGEsjaaktREadonlyGameState

    // ICardGame
    IGesjaaktPlayer PlayerOnTurn { get; }
    
    IReadOnlyDeck<Card> Deck { get; }
    
    bool HasOpenCard { get; }

    int OpenCardValue { get; }

    // Gesjaakt specifiek
    int AmountOfCoinsOnTable { get; }

}