using Microsoft.Extensions.Logging;

namespace Infrastructure.Logging;

public class Logger<T> : Domain.Interfaces.ILogger<T>
{
    private readonly ILogger<T> _logger;

    public Logger(ILogger<T> logger)
    {
        _logger = logger;
    }

    public void LogInformation(string message) => _logger.LogInformation(message);

    public void LogWarning(string message) => _logger.LogWarning(message);

    public void LogError(string message) => _logger.LogError(message);

    public void LogDebug(string message) => _logger.LogDebug(message);

    public void LogCritical(string message) => _logger.LogCritical(message);
}