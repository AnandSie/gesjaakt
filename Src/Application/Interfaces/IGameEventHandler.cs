using Domain.Entities.Events;

namespace Application.Interfaces;

// REFACTOR: create game specific levels which are not similar to logging levels. (e.g. levels: gamechanging, special, ordinary)
public interface IGameEventHandler
{
    public void HandleEvent(object sender, CriticalEvent eventObject);
    public void HandleEvent(object sender, ErrorEvent eventObject);
    public void HandleEvent(object sender, WarningEvent eventObject);
    public void HandleEvent(object sender, InfoEvent eventObject);
}
