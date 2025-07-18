namespace Domain.Interfaces;

// TODO: SPlit in gesjaakt en ordinary
public interface IGameStateWriter
{
    IEnumerable<IGesjaaktActions> Players { get; }
    public void AddPlayer(IGesjaaktActions player);
    public void AddCoinToTable(ICoin coin);
    public IEnumerable<ICoin> TakeCoins();
    public void RemoveCardsFromDeck(int amount);
    public void OpenNextCardFromDeck();
    public ICard TakeOpenCard();
    public void NextPlayer();
}