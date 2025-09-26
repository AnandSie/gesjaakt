using Domain.Entities.Components;
using Domain.Interfaces.Components;
using Domain.Interfaces.Games.TakeFive;

namespace Domain.Entities.Game.TakeFive;

public class TakeFiveReadOnlyGameState : ITakeFiveReadOnlyGameState
{
    private readonly TakeFiveGameState _gameState;
    private readonly IReadOnlyDeck<TakeFiveCard> _readonlyDeck; // FIXME: remove, unused?
    public TakeFiveReadOnlyGameState(TakeFiveGameState gameState)
    {
        _gameState = gameState;
        _readonlyDeck = new ReadOnlyDeck<TakeFiveCard>(gameState.Deck);
    }

    public IEnumerable<IEnumerable<TakeFiveCard>> CardRows => _gameState.CardRows;

    // TODO: Share Call ReadOnly?
    // FIXME: cache -> move from method to local readonly
    public IEnumerable<ITakeFiveReadOnlyPlayer> Players =>_gameState.Players.Select(p => p.AsReadOnly());
}
