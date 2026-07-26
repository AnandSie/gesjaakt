using Application.Interfaces;
using Domain.Entities.Events;

namespace Application.Events;

// The single point every game event passes through.
//
// Recording happens before filtering on purpose: a 10.000-game simulation shows
// almost nothing on screen, but the summary afterwards should still be able to
// say how often players were gesjaakt. Display is a view on the events, not the
// place they are stored.
public class GameEventHub(IGameEventStatistics statistics, IGameEventPresenter presenter) : IGameEventHandler
{
    private EventImportance _minImportance = EventImportance.Ordinary;

    public void SetMinImportance(EventImportance importance) => _minImportance = importance;

    public void HandleEvent(object sender, GameEvent gameEvent)
    {
        statistics.Record(gameEvent);

        if (gameEvent.Importance < _minImportance) return;

        presenter.Present(gameEvent);
    }

    public void ShowSummary() => presenter.PresentSummary(statistics.Report());
}
