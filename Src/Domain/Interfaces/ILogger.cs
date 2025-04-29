using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ILogger<T>
    {
        void LogInformation(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogDebug(string message);
        void LogCritical(string message);
    }
}
