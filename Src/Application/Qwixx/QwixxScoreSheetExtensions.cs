using Domain.Entities.Game.Qwixx;
using Domain.Interfaces.Games.Qwixx;
using System.Text;

namespace Application.Qwixx;

// Presentation-flavored rendering helpers for a Qwixx player's own score sheet, mirroring how
// TakeFiveCardExtensions.ToTableString() renders TakeFive's shared card rows. Kept as extension
// methods rather than a ToString() override, because an accurate render needs the surrounding
// IQwixxReadOnlyGameState too (e.g. whether a color is locked by *another* player) - something
// IQwixxReadOnlyPlayer alone can't know, and QwixxRow is deliberately never exposed to callers
// outside the dealer (see IQwixxReadOnlyPlayer). Lives here in Application, not alongside
// TakeFiveCardExtensions in Domain.Extensions, since ANSI escape codes are terminal-presentation
// concerns that don't belong in the domain layer.
public static class QwixxScoreSheetExtensions
{
    private const string Reset = "\x1b[0m";
    private const string Bold = "\x1b[1m";
    private const string Dim = "\x1b[2m";
    private const string Strikethrough = "\x1b[9m";

    // A different palette from the row colors (red/yellow/green/blue), so a player's name is
    // never confused with a row when both appear in the same line.
    private static readonly string[] PlayerNameColors =
    {
        "\x1b[95m", // magenta
        "\x1b[96m", // cyan
        "\x1b[97m", // white
        "\x1b[90m", // gray
        "\x1b[35m", // magenta (dim) - covers the 5th seat, QwixxRules.MaxNumberOfPlayers
    };

    public static string AnsiColorCode(this QwixxColor color) => color switch
    {
        QwixxColor.Red => "\x1b[91m",
        QwixxColor.Yellow => "\x1b[93m",
        QwixxColor.Green => "\x1b[92m",
        QwixxColor.Blue => "\x1b[94m",
        _ => "",
    };

    // A distinct, stable color per player, based on their seat position in gameState.Players -
    // the same player keeps the same color for the whole game, and no two players (up to the
    // 5-player max) share one.
    //
    // Seats are matched by identity, not by name: manual players type their own names, so two
    // of them can genuinely be called the same thing, and name-matching would then paint both
    // seats the same color. QwixxPlayer.AsReadOnly() hands back one view per player for exactly
    // this reason. A player who isn't seated in this game gets no color rather than the first
    // seat's, which would disguise them as another player.
    public static string AnsiNameColorCode(this IQwixxReadOnlyPlayer player, IQwixxReadOnlyGameState gameState)
    {
        var seatIndex = gameState.Players.ToList().FindIndex(seated => ReferenceEquals(seated, player));
        if (seatIndex < 0)
        {
            return "";
        }

        return PlayerNameColors[seatIndex % PlayerNameColors.Length];
    }

    // Mirrors exactly what QwixxGameDealer.TryMark checks, so a caller can filter out choices
    // that would silently be rejected if picked.
    public static bool CanReallyMark(this IQwixxReadOnlyPlayer player, IQwixxReadOnlyGameState gameState, QwixxColor color, int number)
    {
        return !gameState.IsColorLocked(color) && player.CanMark(color, number);
    }

    // A colored, card-style rendering of a player's own score sheet: all 11 numbers per row in
    // real print order, with marked/skipped/available numbers visually distinct, plus the lock
    // cell and penalty boxes.
    public static string ToScoreSheetString(this IQwixxReadOnlyPlayer player, IQwixxReadOnlyGameState gameState)
    {
        var nameColor = player.AnsiNameColorCode(gameState);
        var sheet = new StringBuilder();
        var rule = new string('=', 46);
        sheet.AppendLine(rule);
        sheet.AppendLine($" 🎲 {Bold}{nameColor}{player.Name}{Reset}{Bold}'s Score Sheet{Reset}  —  Score: {Bold}{player.Score}{Reset}");
        sheet.AppendLine(rule);

        foreach (var color in Enum.GetValues<QwixxColor>())
        {
            sheet.AppendLine(RenderRow(player, gameState, color));
        }

        sheet.AppendLine(new string('-', 46));
        sheet.Append($" Penalties: {RenderPenalties(player)}");
        sheet.AppendLine();
        sheet.Append(rule);

        return sheet.ToString();
    }

    private static string RenderRow(IQwixxReadOnlyPlayer player, IQwixxReadOnlyGameState gameState, QwixxColor color)
    {
        var ascending = color is QwixxColor.Red or QwixxColor.Yellow;
        var numbers = Enumerable.Range(QwixxRules.MinRowNumber, QwixxRules.MaxRowNumber - QwixxRules.MinRowNumber + 1);
        if (!ascending)
        {
            numbers = numbers.Reverse();
        }

        var colorCode = color.AnsiColorCode();
        var cells = string.Join(" ", numbers.Select(number => RenderCell(player, gameState, color, number, colorCode)));
        var label = $"{colorCode}{Bold}{color,-6}{Reset}";
        var lockCell = RenderLockCell(player, gameState, color, colorCode);

        return $" {label} {cells}  {lockCell}";
    }

    private static string RenderCell(IQwixxReadOnlyPlayer player, IQwixxReadOnlyGameState gameState, QwixxColor color, int number, string colorCode)
    {
        var label = number.ToString().PadLeft(2);
        if (player.IsMarked(color, number))
        {
            return $"{colorCode}{Bold}{Strikethrough}{label}{Reset}";
        }
        if (player.CanReallyMark(gameState, color, number))
        {
            return $"{colorCode}{Bold}{label}{Reset}";
        }
        return $"{Dim}{Strikethrough}{label}{Reset}"; // permanently skipped, or the color is locked
    }

    private static string RenderLockCell(IQwixxReadOnlyPlayer player, IQwixxReadOnlyGameState gameState, QwixxColor color, string colorCode)
    {
        if (player.IsRowLocked(color))
        {
            return $"{colorCode}{Bold}🔒{Reset}";
        }
        if (gameState.IsColorLocked(color))
        {
            return $"{Dim}🔒{Reset}"; // locked by another player
        }
        return "[ ]";
    }

    private static string RenderPenalties(IQwixxReadOnlyPlayer player)
    {
        var boxes = Enumerable.Range(0, QwixxRules.MaxPenalties)
            .Select(i => i < player.Penalties ? $"\x1b[91m{Bold}[X]{Reset}" : "[ ]");
        return string.Join(" ", boxes);
    }
}
