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

        logger.LogInformation(summary.ToString());
    }
}
