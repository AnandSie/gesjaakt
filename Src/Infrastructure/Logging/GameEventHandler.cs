using Application.Interfaces;
using Domain.Entities.Events;

namespace Logging;

public class GameEventHandler(ILogger<GameEventHandler> logger) : IGameEventHandler
{
    public void HandleEvent(object sender, CriticalEvent eventObject)
    {
        logger.LogCritical(eventObject.Message);
    }

    public void HandleEvent(object sender, ErrorEvent eventObject)
    {
        logger.LogError(eventObject.Message);
    }

    public void HandleEvent(object sender, WarningEvent eventObject)
    {
        logger.LogWarning(eventObject.Message);
    }

    public void HandleEvent(object sender, InfoEvent eventObject)
    {
        logger.LogInformation(eventObject.Message);
    }
}
