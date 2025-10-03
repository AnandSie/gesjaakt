using Application.Interfaces;
using Domain.Entities.Events;

namespace Logging;

public class GameEventHandler : IGameEventHandler
{
    private readonly ILogger<GameEventHandler> _logger;
    private EventLevel _minLevel;
    private readonly Dictionary<EventLevel, Action<string>> _logActions;

    public GameEventHandler(ILogger<GameEventHandler> logger, EventLevel minLevel = EventLevel.Info)
    {
        _logger = logger;
        _minLevel = minLevel;

        // REFACTOR - consider moving to factory to make this class more closed
        _logActions = new Dictionary<EventLevel, Action<string>>
        {
            [EventLevel.Critical] = _logger.LogCritical,
            [EventLevel.Error] = _logger.LogError,
            [EventLevel.Warning] = _logger.LogWarning,
            [EventLevel.Info] = _logger.LogInformation
        };
    }

    public void SetMinLevel(EventLevel level) => _minLevel = level;

    public void HandleEvent(object sender, BaseEvent eventObject)
    {
        if (eventObject.Level < _minLevel) return;

        if (_logActions.TryGetValue(eventObject.Level, out var log))
        {
            log.Invoke(eventObject.Message);
        }
    }
}
