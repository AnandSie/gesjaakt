using Domain.Entities.Components;
using Domain.Interfaces.Components;
using Domain.Interfaces.Games.Gesjaakt;

namespace Domain.Entities.Game.Gesjaakt;

public class GesjaaktReadOnlyGameState : IGesjaaktReadOnlyGameState
{
    private readonly IGesjaaktGameState _gameState;

    public GesjaaktReadOnlyGameState(IGesjaaktGameState gameState) 
    {
        _gameState = gameState; // FIXME: maybe don't save the state itself to avoid reflection and improve performance
    }

    public IGesjaaktReadOnlyPlayer PlayerOnTurn => _gameState.PlayerOnTurn.AsReadOnly();

    public IReadOnlyDeck<Card> Deck => _gameState.Deck;

    public bool HasOpenCard => _gameState.HasOpenCard;

    public int OpenCardValue => _gameState.OpenCardValue;

    public int AmountOfCoinsOnTable => _gameState.AmountOfCoinsOnTable;

    public IEnumerable<IGesjaaktReadOnlyPlayer> Players => _gameState.Players.Select(p => p.AsReadOnly());

    public override string ToString() => _gameState.ToString() ?? "";
}
