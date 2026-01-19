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
        gameRunner.AllSimItersEnded += gameEventHandler.HandleEvent;
        return this;
    }

    public void DisplayEvent(object sender, BaseEvent eventObject)
    {
        display.UpdateMessage(eventObject.Message);
        gameEventHandler.HandleEvent(sender, eventObject);
    }
}
