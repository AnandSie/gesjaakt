using Domain.Entities.Game.Qwixx;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.Qwixx;

public interface IQwixxGameState : IGameState<IQwixxPlayer>, IToReadOnly<IQwixxReadOnlyGameState>
{
    // QX-007
    IQwixxPlayer PlayerOnTurn { get; }
    void NextPlayer();

    // QX-024/QX-025: once any player locks a row, that color is locked for everyone.
    void LockColor(QwixxColor color);
    bool IsColorLocked(QwixxColor color);

    // QX-026
    bool IsGameOver { get; }
}
