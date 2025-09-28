using Application.Interfaces;

namespace Presentation.ConsoleApp.Helpers;

internal class CLIPlayerInputProvider : IPlayerInputProvider
{
    private readonly ILogger<CLIPlayerInputProvider> _logger;

    public CLIPlayerInputProvider(ILogger<CLIPlayerInputProvider> logger)
    {
        _logger = logger;
    }

    public string GetPlayerInput(string question)
    {
        _logger.LogCritical($"{question}\n");

        while (true)
        {
            var value = Console.ReadLine();
            if (value is null)
            {
                _logger.LogCritical($"Invalid input. Please enter a string.");
            }
            else
            {
                return value;
            }

        }
    }

    private int GetPlayerInputAsInt(IEnumerable<int> allowedInts)
    {
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out var value) && allowedInts.Contains(value))
            {
                return value;
            }

            _logger.LogCritical($"Invalid input. Please enter a valid number from the list {allowedInts}.");
        }
    }

    public int GetPlayerInputAsInt(string question, IEnumerable<int> allowedInts)
    {
        _logger.LogCritical(question);
        return GetPlayerInputAsInt(allowedInts);
    }

    public int GetPlayerInputAsIntWithMinMax(string question, int min, int max)
    {
        _logger.LogCritical(question);
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out var value) && value >= min && value <= max)
            {
                return value;
            }

            _logger.LogCritical($"Invalid input. Please enter a valid number between {min} and {max}.");
        }
    }
}
