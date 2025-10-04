namespace Application;

public class SimulationConfiguration
{
    public double TargetSimulationDurationSeconds { get; } = 20;
    public int NumberOfSimulationsPerPlayerCombination { get; } = 1000;
    public int NumberOfGamesPerSimulation { get; } = 1000;
}
