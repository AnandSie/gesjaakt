namespace Domain.Interfaces;

// FIXME: DIt is al een gesjaakt GameState.., wat is de echte IGameState (... Plalyers? PLayerOnTurn?)
public interface IGameStateReader
{
    IEnumerable<IPlayerState> Players { get; }
    IPlayerState PlayerOnTurn { get; }
    int OpenCardValue { get; }
    bool HasOpenCard {  get; }
    IDeckState Deck { get; }
    int AmountOfCoinsOnTable { get; }
}
