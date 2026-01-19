using Microsoft.Extensions.Logging;

namespace Infrastructure.Logging;

public class Logger<T>(ILogger<T> logger) : Application.Interfaces.ILogger<T>
{
    public void LogInformation(string message) => logger.LogInformation(message);

    public void LogWarning(string message) => logger.LogWarning(message);

    public void LogError(string message) => logger.LogError(message);

    public void LogDebug(string message) => logger.LogDebug(message);

    public void LogCritical(string message) => logger.LogCritical(message);
}