using Application.Interfaces;
using Application.Qwixx;
using Domain.Entities.Game.Qwixx;
using Domain.Interfaces.Games.Qwixx;
using System.Text;

namespace Application.Qwixx.Thinkers;

public class ManualQwixxThinker(IPlayerInputProvider playerInputProvider, string name) : BaseQwixxThinker
{
    private const string Reset = "\x1b[0m";

    public override string Name => name;

    public override QwixxColor? DecideWhiteMark(IQwixxReadOnlyGameState gameState, int whiteSum)
    {
        // ILogger strips ANSI color codes from message content (see IPlayerInputProvider below),
        // so the colored score sheet is written straight to the console instead.
        Console.WriteLine(Me.ToScoreSheetString(gameState));

        // Only offer rows where whiteSum is actually markable right now - otherwise the menu
        // lists a "choice" that silently does nothing when picked, which is just confusing.
        var options = Enum.GetValues<QwixxColor>()
            .Where(color => Me.CanReallyMark(gameState, color, whiteSum))
            .ToList();

        var lines = new StringBuilder();
        for (var i = 0; i < options.Count; i++)
        {
            lines.AppendLine($"{i + 1}. {options[i].AnsiColorCode()}{options[i]}{Reset}");
        }
        var skipOption = options.Count + 1;
        lines.AppendLine($"{skipOption}. Skip");
        Console.WriteLine(lines.ToString());

        var question = $"Hi {name}, the white-dice sum is {whiteSum}. Which row do you want to mark it in (or {skipOption} to skip)?";
        var choice = playerInputProvider.GetPlayerInputAsInt(question, Enumerable.Range(1, skipOption));
        return choice == skipOption ? null : options[choice - 1];
    }

    public override QwixxMark? DecideColoredMark(IQwixxReadOnlyGameState gameState, QwixxDiceRoll roll)
    {
        // Same filtering as DecideWhiteMark - and de-duplicated, since a double white roll can
        // legitimately produce the same candidate sum twice (QX-010), which would otherwise show
        // as two identical-looking options.
        var candidates = Enum.GetValues<QwixxColor>()
            .SelectMany(color => roll.ColoredSums(color).Select(sum => new QwixxMark(color, sum)))
            .Where(mark => Me.CanReallyMark(gameState, mark.Color, mark.Number))
            .Distinct()
            .ToList();

        Console.WriteLine(Me.ToScoreSheetString(gameState));
        var candidateLines = new StringBuilder();
        for (var i = 0; i < candidates.Count; i++)
        {
            candidateLines.AppendLine($"{i + 1}. {candidates[i].Color.AnsiColorCode()}{candidates[i].Color} {candidates[i].Number}{Reset}");
        }
        var skipOption = candidates.Count + 1;
        candidateLines.AppendLine($"{skipOption}. Skip");
        Console.WriteLine(candidateLines.ToString());

        var question = $"Hi {name}, pick a colored combination to mark, or {skipOption} to skip:";
        var choice = playerInputProvider.GetPlayerInputAsInt(question, Enumerable.Range(1, skipOption));
        return choice == skipOption ? null : candidates[choice - 1];
    }

    public override bool DecideToLock(IQwixxReadOnlyGameState gameState, QwixxColor color)
    {
        Console.WriteLine(Me.ToScoreSheetString(gameState));

        var question = $"Hi {name}, you can lock the {color} row now. Do you want to lock it?\n1. Yes  2. No";
        var choice = playerInputProvider.GetPlayerInputAsInt(question, [1, 2]);
        return choice == 1;
    }
}
