using Domain.Entities.Events;

namespace Application.Interfaces;

// What the *EventCollector classes attach every game event to. Records the
// event for the end-of-run summary, then hands it to a presenter if it clears
// the configured importance threshold.
public interface IGameEventHandler
{
    void HandleEvent(object sender, GameEvent gameEvent);

    // The granularity knob: Ordinary shows the full narration of a manual game,
    // GameChanging keeps a 10.000-game simulation quiet (and fast).
    void SetMinImportance(EventImportance importance);

    void ShowSummary();
}
