using Domain.Interfaces.Components;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.Gesjaakt;

// TODO: SPlit in gesjaakt en ordinary
public interface IGesjaaktMutableGameState: IMutableGameState<IGesjaaktPlayer>
{
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
}