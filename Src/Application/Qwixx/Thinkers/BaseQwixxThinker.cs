using Domain.Entities.Game.Qwixx;
using Domain.Interfaces.Games.Qwixx;

namespace Application.Qwixx.Thinkers;

// Mirrors BaseTakeFiveThinker: QwixxPlayer pushes the thinker's own current read-only state in
// via SetState before every decision, so subclasses don't need to hunt for "which player am I"
// in the game state themselves (e.g. via a Name-based lookup).
public abstract class BaseQwixxThinker : IQwixxThinker
{
    protected IQwixxReadOnlyPlayer Me { get; private set; } = null!;

    public abstract string Name { get; }

    public abstract QwixxColor? DecideWhiteMark(IQwixxReadOnlyGameState gameState, int whiteSum);

    public abstract QwixxMark? DecideColoredMark(IQwixxReadOnlyGameState gameState, QwixxDiceRoll roll);

    public abstract bool DecideToLock(IQwixxReadOnlyGameState gameState, QwixxColor color);

    public void SetState(IQwixxReadOnlyPlayer state)
    {
        Me = state;
    }
}
