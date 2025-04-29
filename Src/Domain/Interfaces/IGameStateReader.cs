namespace Domain.Interfaces;

public interface IGameStateReader
{
    IEnumerable<IPlayerState> Players { get; }
    IPlayerState PlayerOnTurn { get; }
    int OpenCardValue { get; }
    bool HasOpenCard {  get; }
    IDeckState Deck { get; }
    int AmountOfCoinsOnTable { get; }
}
