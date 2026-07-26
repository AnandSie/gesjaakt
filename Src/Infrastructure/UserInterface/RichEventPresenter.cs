using Application.Events;
using Application.Interfaces;
using Domain.Entities.Events;

namespace UserInterface;

// Renders game events as a readable narration instead of log records.
//
// Every line is:  <glyph> <kind>            <message>
// with the glyph and kind coloured by importance/category, so the shape of a
// game is visible at a glance and nothing pretends to be an "error" just to
// clear a log-level threshold.
public class RichEventPresenter : IGameEventPresenter
{
    // Widest event kind we currently raise is "PlayerCombinationIterStarting";
    // clamping keeps the message column aligned without letting one long name
    // push everything off screen.
    private const int KindColumnWidth = 22;

    private readonly EventStatisticsRenderer _summaryRenderer = new();

    public void Present(GameEvent gameEvent)
    {
        var glyph = EventTheme.Colored(EventTheme.GlyphFor(gameEvent), gameEvent);
        var kind = EventTheme.Colored(Truncate(Humanize(gameEvent.Kind), KindColumnWidth), gameEvent);

        foreach (var (line, index) in Lines(gameEvent.Message))
        {
            // Continuation lines keep the gutter but drop the repeated kind, the
            // way a wrapped commit message hangs under its subject.
            var prefix = index == 0
                ? $" {glyph} {Ansi.PadVisibleRight(kind, KindColumnWidth)} "
                : $" {EventTheme.Colored(Ansi.Gutter, gameEvent)} {new string(' ', KindColumnWidth)} ";

            Console.WriteLine(prefix + line);
        }
    }

    public void PresentSummary(EventStatisticsReport report) => _summaryRenderer.Render(report);

    private static IEnumerable<(string Line, int Index)> Lines(string message)
    {
        var lines = message.Replace("\r\n", "\n").TrimEnd('\n').Split('\n');
        return lines.Select((line, index) => (line.TrimEnd(), index));
    }

    // "PlayerCombinationIterEnded" reads better as "Player Combination Iter Ended"
    // once it is a label rather than an identifier.
    private static string Humanize(string kind)
    {
        var spaced = string.Concat(kind.Select((c, i) =>
            i > 0 && char.IsUpper(c) && !char.IsUpper(kind[i - 1]) ? " " + c : c.ToString()));

        return spaced;
    }

    private static string Truncate(string text, int width) =>
        text.Length <= width ? text : text[..(width - 1)] + Ansi.Glyph("…", ".");
}
