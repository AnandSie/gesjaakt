using Domain.Interfaces.Games.Qwixx;

namespace Domain.Entities.Game.Qwixx;

public class QwixxReadOnlyGameState : IQwixxReadOnlyGameState
{
    private readonly IQwixxGameState _gameState;

    public QwixxReadOnlyGameState(IQwixxGameState gameState)
    {
        _gameState = gameState;
    }

    public IEnumerable<IQwixxReadOnlyPlayer> Players => _gameState.Players.Select(p => p.AsReadOnly());

    public IQwixxReadOnlyPlayer PlayerOnTurn => _gameState.PlayerOnTurn.AsReadOnly();

    public bool IsColorLocked(QwixxColor color) => _gameState.IsColorLocked(color);

    public bool IsGameOver => _gameState.IsGameOver;
}
