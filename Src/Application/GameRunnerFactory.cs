using Application.Interfaces;

namespace Application;

public class GameRunnerFactory(Dictionary<Type, Func<IGameRunner>> factories)
{
    public IGameRunner Create(Type playerType)
    {
        if (!factories.TryGetValue(playerType, out var factory))
        {
            throw new ArgumentException($"Unknown player type {playerType}");
        }

        return factory.Invoke();
    }
}
