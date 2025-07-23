using Domain.Entities.Components;
using Domain.Interfaces.Components;
using Domain.Interfaces.Games.BaseGame;
using Domain.Interfaces.Games.Gesjaakt;

namespace Domain.Interfaces;

public interface IGesjaaktReadOnlyGameState: IReadOnlyGameState<IGesjaaktPlayerState>
{
    // IGamePlayerByPlayer (takeFive, all together)
    IGesjaaktPlayerState PlayerOnTurn { get; }
    
    // ICardGame
    IReadOnlyDeck<Card> Deck { get; }
    int OpenCardValue { get; }
    bool HasOpenCard {  get; }

    // Gesjaakt specifiek
    int AmountOfCoinsOnTable { get; }
}
