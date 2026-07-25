using Domain.Entities.Game.Qwixx;
using Domain.Interfaces.Games.Qwixx;

namespace Application.Qwixx.Thinkers;

// A simple default bot: marks the first row it can (in Red/Yellow/Green/Blue order) and always
// locks when it gets the chance. Not a competitive strategy, just a working example bot so the
// game is playable out of the box - hackathon participants are expected to do better.
public class GreedyQwixxThinker(string name) : BaseQwixxThinker
{
    public override string Name { get; } = name;

    public override QwixxColor? DecideWhiteMark(IQwixxReadOnlyGameState gameState, int whiteSum)
    {
        foreach (var color in Enum.GetValues<QwixxColor>())
        {
            if (!gameState.IsColorLocked(color) && Me.CanMark(color, whiteSum))
            {
                return color;
            }
        }

        return null;
    }

    public override QwixxMark? DecideColoredMark(IQwixxReadOnlyGameState gameState, QwixxDiceRoll roll)
    {
        foreach (var color in Enum.GetValues<QwixxColor>())
        {
            if (gameState.IsColorLocked(color))
            {
                continue;
            }

            foreach (var sum in roll.ColoredSums(color))
            {
                if (Me.CanMark(color, sum))
                {
                    return new QwixxMark(color, sum);
                }
            }
        }

        return null;
    }

    public override bool DecideToLock(IQwixxReadOnlyGameState gameState, QwixxColor color) => true;
}
