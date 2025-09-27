// TODO: move out domain?
namespace Domain.Interfaces;

// TODO: move to application layer
// -> the application sometimes wants to log stuff (and is implemented in infrastructure). Domain should not depend on logger
public interface ILogger<T>
{
    void LogInformation(string message);
    void LogWarning(string message);
    void LogError(string message);
    void LogDebug(string message);
    void LogCritical(string message);
}
