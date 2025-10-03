using Application.Interfaces;

namespace Application;

public class GameRunnerEventCollector(IGameEventHandler gameEventHandler) : IGameRunnerEventCollector
{
    public IGameRunnerEventCollector Attach(IGameRunner gameRunner)
    {
        gameRunner.GameEnded += gameEventHandler.HandleEvent;
        gameRunner.GameSimulationStarting += gameEventHandler.HandleEvent;
        gameRunner.GameSimulationEnded += gameEventHandler.HandleEvent;
        gameRunner.PlayerCombinationSimulationStarting += gameEventHandler.HandleEvent;
        gameRunner.PlayerCombinationSimulationEnded += gameEventHandler.HandleEvent;
        return this;
    }
}
