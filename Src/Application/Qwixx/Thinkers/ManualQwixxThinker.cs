using Application.Interfaces;
using Domain.Entities.Game.Qwixx;
using Domain.Interfaces.Games.Qwixx;
using System.Text;

namespace Application.Qwixx.Thinkers;

public class ManualQwixxThinker(IPlayerInputProvider playerInputProvider, string name) : BaseQwixxThinker
{
    public override string Name => name;

    public override QwixxColor? DecideWhiteMark(IQwixxReadOnlyGameState gameState, int whiteSum)
    {
        var question = new StringBuilder();
        question.AppendLine(RenderMyScoreSheet(gameState));
        question.AppendLine($"Hi {name}, the white-dice sum is {whiteSum}. Which row do you want to mark it in?");
        question.AppendLine("1. Red  2. Yellow  3. Green  4. Blue  5. Skip");

        var choice = playerInputProvider.GetPlayerInputAsInt(question.ToString(), [1, 2, 3, 4, 5]);
        return choice switch
        {
            1 => QwixxColor.Red,
            2 => QwixxColor.Yellow,
            3 => QwixxColor.Green,
            4 => QwixxColor.Blue,
            _ => null,
        };
    }

    public override QwixxMark? DecideColoredMark(IQwixxReadOnlyGameState gameState, QwixxDiceRoll roll)
    {
        var candidates = Enum.GetValues<QwixxColor>()
            .SelectMany(color => roll.ColoredSums(color).Select(sum => new QwixxMark(color, sum)))
            .ToList();

        var question = new StringBuilder();
        question.AppendLine(RenderMyScoreSheet(gameState));
        question.AppendLine($"Hi {name}, pick a colored combination to mark, or skip:");
        for (var i = 0; i < candidates.Count; i++)
        {
            question.AppendLine($"{i + 1}. {candidates[i].Color} {candidates[i].Number}");
        }
        var skipOption = candidates.Count + 1;
        question.AppendLine($"{skipOption}. Skip");

        var choice = playerInputProvider.GetPlayerInputAsInt(question.ToString(), Enumerable.Range(1, skipOption));
        return choice == skipOption ? null : candidates[choice - 1];
    }

    public override bool DecideToLock(IQwixxReadOnlyGameState gameState, QwixxColor color)
    {
        var question = $"Hi {name}, you can lock the {color} row now. Do you want to lock it?\n1. Yes  2. No";
        var choice = playerInputProvider.GetPlayerInputAsInt(question, [1, 2]);
        return choice == 1;
    }

    private string RenderMyScoreSheet(IQwixxReadOnlyGameState gameState)
    {
        var sheet = new StringBuilder();
        sheet.AppendLine("---YOUR SCORE SHEET---");
        foreach (var color in Enum.GetValues<QwixxColor>())
        {
            var lockNote = Me.IsRowLocked(color) ? " (locked)" : gameState.IsColorLocked(color) ? " (locked by another player)" : "";
            sheet.AppendLine($"{color}: {Me.MarkedCount(color)} marks{lockNote}");
        }
        sheet.AppendLine($"Penalties: {Me.Penalties}");
        return sheet.ToString();
    }
}
