namespace Application.Interfaces;

public interface ILogger<T>
{
    void LogInformation(string message);
    void LogWarning(string message);
    void LogError(string message);
    void LogDebug(string message);
    void LogCritical(string message);
}
