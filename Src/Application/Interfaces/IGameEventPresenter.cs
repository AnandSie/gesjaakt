using Application.Events;
using Domain.Entities.Events;

namespace Application.Interfaces;

// Turns events into something a human sees. Implementations decide *how* it
// looks (rich ANSI, plain log lines); the importance filter that decides
// *whether* an event gets here at all lives in the event hub.
public interface IGameEventPresenter
{
    void Present(GameEvent gameEvent);

    // Called once when a run finishes, with the aggregate over every event that
    // was raised - not just the ones that passed the importance filter.
    void PresentSummary(EventStatisticsReport report);
}
