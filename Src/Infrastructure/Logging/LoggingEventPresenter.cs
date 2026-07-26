using System.Text;
using Application.Events;
using Application.Interfaces;
using Domain.Entities.Events;

namespace Logging;

// Plain-text presenter for consoles that can't do cursor tricks or colour
// (--simple-console, CI, piped output).
//
// The log level here is a rendering choice only - it says how loud the line
// should be, not what the event means. That distinction now lives on the event
// itself as EventImportance/EventCategory.
public class LoggingEventPresenter(ILogger<LoggingEventPresenter> logger) : IGameEventPresenter
{
    public void Present(GameEvent gameEvent)
    {
        var line = $"[{gameEvent.Kind}] {gameEvent.Message}";

        if (gameEvent.Category == EventCategory.Fault)
        {
            logger.LogError(line);
            return;
        }

        switch (gameEvent.Importance)
        {
            case EventImportance.GameChanging:
                logger.LogWarning(line);
                break;
            case EventImportance.Special:
            case EventImportance.Notable:
                logger.LogInformation(line);
                break;
            default:
                logger.LogDebug(line);
                break;
        }
    }

    public void PresentSummary(EventStatisticsReport report)
    {
        if (report.IsEmpty)
        {
            logger.LogInformation("No events were recorded.");
            return;
        }

        var summary = new StringBuilder();
        summary.AppendLine($"Event summary over {report.GamesObserved} game(s) - {report.TotalGameEvents} events:");

        foreach (var statistic in report.GameStatistics)
        {
            summary.Append($"\t{statistic.Kind}: {statistic.Count} total, {statistic.PerGame(report.GamesObserved):N2} per game");

            if (statistic.HasValues)
            {
                summary.Append($", mean {statistic.ValueMean:N2} (min {statistic.Minimum:N2}, max {statistic.Maximum:N2})");
            }

            summary.AppendLine();
        }

        AppendPlayerBreakdown(summary, report);

        logger.LogInformation(summary.ToString());

        WarnAboutAttributionGap(report);
    }

    // Logged separately, and at Error, so it survives whatever the console is
    // doing to the big informational block above it.
    private void WarnAboutAttributionGap(EventStatisticsReport report)
    {
        if (!report.HasAttributionGap) return;

        var alarm = new StringBuilder();
        alarm.AppendLine("INCOMPLETE EVENT ATTRIBUTION - the per-player shares above are understated.");

        foreach (var statistic in report.IncompletelyAttributed)
        {
            alarm.AppendLine($"\t{statistic.Kind}: {statistic.UnattributedCount} of {statistic.Count} events named nobody"
                + $" ({(double)statistic.UnattributedCount / statistic.Count * 100:N1}%)");
        }

        alarm.Append("Player shares divide by EVERY event of the kind, so those rows do not add up to 100%. "
            + "This is a bug in the raise site: pass actor: at every ?.Invoke for the kinds listed here.");

        logger.LogError(alarm.ToString());
    }

    // Shares rather than counts, for the same reason as the rich renderer: not
    // every player is in every game, so a raw count mostly measures who was dealt
    // in most often.
    private static void AppendPlayerBreakdown(StringBuilder summary, EventStatisticsReport report)
    {
        if (!report.HasPlayerBreakdown) return;

        summary.AppendLine("Events per player (share of each event):");

        foreach (var statistic in report.ActorStatistics)
        {
            var shares = report.Players
                .Where(player => statistic.CountFor(player) > 0)
                .Select(player => $"{player} {(double)statistic.CountFor(player) / statistic.Count * 100:N1}%");

            summary.AppendLine($"\t{statistic.Kind}: {string.Join(", ", shares)}");
        }
    }
}
