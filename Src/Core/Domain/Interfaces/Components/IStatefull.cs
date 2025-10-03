namespace Domain.Interfaces.Components;

public interface IStatefull<TState>
{
    void SetState(TState state);
}
