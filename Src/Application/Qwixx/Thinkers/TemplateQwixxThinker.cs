using Domain.Entities.Game.Qwixx;
using Domain.Interfaces.Games.Qwixx;

namespace Application.Qwixx.Thinkers;

public class TemplateQwixxThinker : BaseQwixxThinker
{
    public override string Name => throw new NotImplementedException();

    // INSTRUCTIONS:
    // 1. Rename this class to your own Thinker e.g. "HarryQwixxThinker"
    // 2. Give it a cool Name
    // 3. Implement the three Decide methods below - see docs/qwixx/rules.md for the QX-### rules
    //    each one is responsible for, and IQwixxThinker for the exact contract of each.
    //    The protected `Me` property (see BaseQwixxThinker) is your own current read-only state -
    //    use it to check what you've already marked before deciding.
    // 4. Register your thinker wherever Qwixx players are created for a game/simulation
    // 5. Play

    // QX-009: called for every player, every turn. Return the color of the row you want to
    // mark `whiteSum` in, or null to skip marking this turn.
    public override QwixxColor? DecideWhiteMark(IQwixxReadOnlyGameState gameState, int whiteSum)
    {
        throw new NotImplementedException();
    }

    // QX-010: called only when you are the active (rolling) player. Use roll.ColoredSums(color)
    // to see the candidate sums for each color, and return the color+number you want to mark,
    // or null to skip.
    public override QwixxMark? DecideColoredMark(IQwixxReadOnlyGameState gameState, QwixxDiceRoll roll)
    {
        throw new NotImplementedException();
    }

    // QX-022/QX-023: called only when the mark you just chose makes locking that row possible.
    // Return true to lock it (removing that color for everyone for the rest of the game),
    // false to leave it open.
    public override bool DecideToLock(IQwixxReadOnlyGameState gameState, QwixxColor color)
    {
        throw new NotImplementedException();
    }
}
