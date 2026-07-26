using Application.Interfaces;
using Domain.Entities.Events;

namespace Application;

public class GameRunnerEventCollector(IGameEventHandler gameEventHandler, IDisplay display) : IGameRunnerEventCollector
{
    public IGameRunnerEventCollector Attach(IGameRunner gameRunner)
    {
        // REFACTOR - UNITTESTS
        gameRunner.GameEnded += gameEventHandler.HandleEvent;
        gameRunner.SimIterStarting += gameEventHandler.HandleEvent;
        gameRunner.SimIterEnded += DisplayEvent;
        gameRunner.PlayerCombinationIterStarting += gameEventHandler.HandleEvent;
        gameRunner.PlayerCombinationIterEnded += gameEventHandler.HandleEvent;
        gameRunner.AllSimItersEnded += FinishEvent;
        return this;
    }

    // Clears before forwarding, so the boxed results land where the live
    // standings were instead of below a stale copy of themselves.
    public void FinishEvent(object sender, GameEvent eventObject)
    {
        display.Clear();
        gameEventHandler.HandleEvent(sender, eventObject);
    }

    public void DisplayEvent(object sender, GameEvent eventObject)
    {
        display.UpdateMessage(eventObject.Message);
        gameEventHandler.HandleEvent(sender, eventObject);
    }
}
