using Domain.Entities.Game.Qwixx;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Interfaces.Games.Qwixx;

public interface IQwixxReadOnlyGameState : IReadOnlyGameState<IQwixxReadOnlyPlayer>
{
    // QX-007
    IQwixxReadOnlyPlayer PlayerOnTurn { get; }

    // QX-024/QX-025: once any player locks a row, that color is locked for everyone.
    bool IsColorLocked(QwixxColor color);

    // QX-026
    bool IsGameOver { get; }
}
