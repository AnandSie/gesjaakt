using Domain.Interfaces;

namespace ConsoleApp.Helpers;

internal class ConsoleInputService : IPlayerInputProvider
{
    private readonly ILogger<ConsoleInputService> _logger;

    public ConsoleInputService(ILogger<ConsoleInputService> logger)
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

    public bool GetPlayerInputForYesNo()
    {
        _logger.LogCritical("1. Yes");
        _logger.LogCritical("2. No");

        var input = GetPlayerInputAsInt(new[] { 1, 2 });

        return input switch
        {
            1 => true,
            2 => false,
            _ => throw new Exception("Unexpected possible"),
        };
    }

    public int GetPlayerInputAsInt(IEnumerable<int> allowedInts)
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
