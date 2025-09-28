namespace Application.Interfaces;

public interface ILogger<T>
{
    void LogCritical(string message);
    void LogError(string message);
    void LogWarning(string message);
    void LogInformation(string message);
    void LogDebug(string message);
}
