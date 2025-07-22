using Domain.Entities.Components;
using Domain.Interfaces.Components;
using Domain.Interfaces.Games.TakeFive;

namespace Domain.Entities.Game.TakeFive;

public class TakeFiveReadOnlyGameState : ITakeFiveReadOnlyGameState
{
    private readonly TakeFiveGameState _gameState;
    private readonly IDeckState _readonlyDeck;
    public TakeFiveReadOnlyGameState(TakeFiveGameState gameState)
    {
        _gameState = gameState;
        _readonlyDeck = new ReadOnlyDeck(gameState.Deck);
    }

    public IDeckState Deck => _readonlyDeck;
    public IEnumerable<IEnumerable<ICard>> CardRows => _gameState.CardRows;

    public IEnumerable<ITakeFivePlayerState> Players =>_gameState.Players;
}
