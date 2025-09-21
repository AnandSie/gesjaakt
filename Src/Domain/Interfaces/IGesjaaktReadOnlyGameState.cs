using Domain.Entities.Components;
using Domain.Interfaces.Components;
using Domain.Interfaces.Games.BaseGame;
using Domain.Interfaces.Games.Gesjaakt;

namespace Domain.Interfaces;

public interface IGesjaaktReadOnlyGameState: IReadOnlyGameState<IGesjaaktReadOnlyPlayer>
{
    // IGamePlayerByPlayer (takeFive, all together)
    IGesjaaktReadOnlyPlayer PlayerOnTurn { get; }
    
    // ICardGame
    IReadOnlyDeck<Card> Deck { get; }
    bool HasOpenCard {  get; }
    int OpenCardValue { get; }

    // Gesjaakt specifiek
    int AmountOfCoinsOnTable { get; }
}
