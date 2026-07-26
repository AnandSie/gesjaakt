using System.Text;
using Application.Events;
using Domain.Entities.Events;

namespace UserInterface;

// Draws the end-of-run event summary as a boxed table: what happened, how
// often, how often per game, and - for the events that carry a number - the
// mean/min/max of that number.
public class EventStatisticsRenderer
{
    private const int MinTableWidth = 72;
    private const int MaxTableWidth = 110;
    private const int BarWidth = 12;
    private const int NameWidth = 26;
    private const int NumberWidth = 10;

    // The per-player table drops the bar column, so its event names get the space
    // the bars used to take.
    private const int KindWidth = 26;
    private const int PlayerWidth = 10;
    private const int MaxPlayerTableWidth = 160;

    public void Render(EventStatisticsReport report)
    {
        if (report.IsEmpty)
        {
            Console.WriteLine();
            Console.WriteLine(Ansi.Dim("  No events were recorded."));
            return;
        }

        var rows = BuildRows(report);
        rows.Add(null);
        rows.Add(Ansi.Dim($"{report.TotalGameEvents:N0} events across {report.GameStatistics.Count} kinds"));

        Console.WriteLine();
        ConsoleBox.Draw($"Event summary over {report.GamesObserved} game(s)", rows,
            minWidth: MinTableWidth, maxWidth: MaxTableWidth);

        RenderPlayerBreakdown(report);
        Console.WriteLine();
    }

    // Second table: of everything that happened, who did it happen to.
    //
    // Cells are each player's share of that row's events rather than a raw count,
    // because "simulate all combinations" does not put every player in every game
    // - a count would just reward whoever was dealt in most often, while a share
    // answers the question a participant actually has: is this happening to my bot
    // more than to the others?
    private static void RenderPlayerBreakdown(EventStatisticsReport report)
    {
        if (!report.HasPlayerBreakdown) return;

        RenderAttributionGap(report);

        // Only shown when something is actually missing. A column reading "-" on
        // every row of every correct run is noise, and the banner above guarantees
        // a real gap can't be scrolled past.
        bool showUnattributed = report.HasAttributionGap;

        int columns = FittingPlayerColumns(report.Players.Count, showUnattributed);
        var players = report.Players.Take(columns).ToList();
        int width = 4 + KindWidth + NumberWidth + (players.Count * PlayerWidth)
            + (showUnattributed ? PlayerWidth : 0);

        var rows = new List<string?>();

        var header = new StringBuilder();
        header.Append("EVENT".PadRight(KindWidth));
        header.Append("TOTAL".PadLeft(NumberWidth));
        foreach (var player in players)
        {
            header.Append(Fit(player, PlayerWidth - 1).PadLeft(PlayerWidth));
        }
        if (showUnattributed)
        {
            header.Append("NOBODY".PadLeft(PlayerWidth));
        }

        rows.Add(Ansi.Dim(header.ToString()));

        foreach (var statistic in report.ActorStatistics)
        {
            var line = new StringBuilder();
            line.Append(Ansi.PadVisibleRight(
                EventTheme.Colored(Fit(statistic.Kind, KindWidth - 1), statistic.Importance, statistic.Category),
                KindWidth));
            line.Append(statistic.Count.ToString("N0").PadLeft(NumberWidth));

            foreach (var player in players)
            {
                line.Append(Share(statistic.CountFor(player), statistic.Count).PadLeft(PlayerWidth));
            }

            if (showUnattributed)
            {
                var gap = Share(statistic.UnattributedCount, statistic.Count);
                line.Append(Ansi.PadVisibleLeft(statistic.HasUnattributed ? Alarm(gap) : gap, PlayerWidth));
            }

            rows.Add(line.ToString());
        }

        int hidden = report.Players.Count - players.Count;
        if (hidden > 0)
        {
            rows.Add(Ansi.Dim($"{hidden} more player(s) not shown - the console is too narrow"));
        }

        Console.WriteLine();
        ConsoleBox.Draw("Events per player (share of each event)", rows, minWidth: width, maxWidth: MaxPlayerTableWidth);
    }

    // The assert. An event kind that names a player at one raise site and not at
    // another produces a table that is quietly wrong rather than obviously broken,
    // so it gets a red box of its own above the table it corrupts.
    private static void RenderAttributionGap(EventStatisticsReport report)
    {
        if (!report.HasAttributionGap) return;

        var rows = report.IncompletelyAttributed
            .Select(s => $"{Fit(s.Kind, KindWidth - 1).PadRight(KindWidth)}"
                       + $"{s.UnattributedCount:N0} of {s.Count:N0} events named nobody"
                       + $" ({(double)s.UnattributedCount / s.Count * 100:N1}%)")
            .ToList();

        var explanation = new[]
        {
            "Player shares divide by EVERY event of the kind, so the rows below do",
            "NOT add up to 100% and understate every player. This is a bug in the",
            "raise site, not a fact about the game: pass actor: at every ?.Invoke",
            "for the kinds listed here.",
        };

        var body = new List<string?>();
        body.AddRange(rows.Select(Alarm));
        body.Add(string.Empty);
        body.AddRange(explanation.Select(Ansi.Dim));

        Console.WriteLine();
        ConsoleBox.Draw($"{Ansi.Glyph("!!", "!!")} INCOMPLETE EVENT ATTRIBUTION", body,
            frameStyle: Alarm, minWidth: MinTableWidth, maxWidth: MaxPlayerTableWidth, wrapLongRows: true);
    }

    private static string Alarm(string text) => Ansi.Rgb(Ansi.Bold(text), 224, 96, 96);

    // A player who never triggered the event shows nothing at all, so the ones who
    // did stand out instead of being buried in a column of 0,0%.
    private static string Share(long forPlayer, long total) =>
        forPlayer == 0 || total == 0 ? "-" : $"{(double)forPlayer / total * 100:N1}%";

    private static int FittingPlayerColumns(int playerCount, bool reserveUnattributedColumn)
    {
        int available = ConsoleWidth() - 4 - KindWidth - NumberWidth
            - (reserveUnattributedColumn ? PlayerWidth : 0);
        int fits = Math.Max(available / PlayerWidth, 1);

        return Math.Min(playerCount, fits);
    }

    private static int ConsoleWidth()
    {
        try
        {
            // Redirected output reports a width, but not one that means anything;
            // assume a roomy terminal so the table isn't needlessly truncated.
            return Console.IsOutputRedirected ? MaxPlayerTableWidth : Math.Min(Console.WindowWidth - 1, MaxPlayerTableWidth);
        }
        catch (IOException)
        {
            return MaxPlayerTableWidth;
        }
    }

    private static List<string?> BuildRows(EventStatisticsReport report)
    {
        long busiest = report.GameStatistics.Max(s => s.Count);

        var header = string.Concat(
            "".PadRight(BarWidth + 1),
            "EVENT".PadRight(NameWidth),
            "COUNT".PadLeft(NumberWidth),
            "/GAME".PadLeft(NumberWidth),
            "MEAN".PadLeft(NumberWidth),
            "MIN".PadLeft(NumberWidth),
            "MAX".PadLeft(NumberWidth));

        var rows = new List<string?> { Ansi.Dim(header) };

        foreach (var statistic in report.GameStatistics)
        {
            var bar = Bar(statistic.Count, busiest, statistic.Importance, statistic.Category);
            var name = EventTheme.Colored(Fit(statistic.Kind, NameWidth - 1), statistic.Importance, statistic.Category);

            var line = new StringBuilder();
            line.Append(bar).Append(' ');
            line.Append(Ansi.PadVisibleRight(name, NameWidth));
            line.Append(statistic.Count.ToString("N0").PadLeft(NumberWidth));
            line.Append(statistic.PerGame(report.GamesObserved).ToString("N2").PadLeft(NumberWidth));
            line.Append(Number(statistic.ValueMean).PadLeft(NumberWidth));
            line.Append(Number(statistic.Minimum).PadLeft(NumberWidth));
            line.Append(Number(statistic.Maximum).PadLeft(NumberWidth));

            rows.Add(line.ToString());
        }

        return rows;
    }

    // Relative frequency bar, so the shape of a run is readable without doing
    // arithmetic on the count column.
    private static string Bar(long count, long busiest, EventImportance importance, EventCategory category)
    {
        int filled = busiest == 0 ? 0 : (int)Math.Round((double)count / busiest * BarWidth);
        filled = Math.Clamp(filled, count > 0 ? 1 : 0, BarWidth);

        var bar = string.Concat(Enumerable.Repeat(Ansi.BarFull, filled));
        var rest = string.Concat(Enumerable.Repeat(Ansi.BarEmpty, BarWidth - filled));

        return EventTheme.Colored(bar, importance, category) + Ansi.Dim(rest);
    }

    // Events that carry no number leave the numeric columns visibly empty rather
    // than showing a misleading 0.
    private static string Number(double? value) => value is double v ? v.ToString("N2") : "-";

    private static string Fit(string text, int width) =>
        text.Length <= width ? text : text[..(width - 1)] + Ansi.Glyph("…", ".");

}
