namespace Application.Interfaces;

public interface IGameRunnerEventCollector
{
    public IGameRunnerEventCollector Attach(IGameRunner gameRunner);

}