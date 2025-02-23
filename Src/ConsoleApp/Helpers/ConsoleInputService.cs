using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Helpers;

internal class ConsoleInputService : IPlayerInputProvider
{
    private readonly ILogger _logger;

    // TODO: Readline
    public ConsoleInputService(ILogger logger)
    {
        _logger = logger;
    }

    public string GetPlayerInput(string question)
    {
        _logger.LogInformation($"{question}\n");

        while (true)
        {
            var value = Console.ReadLine();
            if (value is null)
            {
                _logger.LogInformation($"Invalid input. Please enter a string.");
            }
            else
            {
                return value;
            }

        }
    }

    public bool GetPlayerInputForYesNo()
    {
        _logger.LogInformation("1. Yes");
        _logger.LogInformation("2. No");

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

            _logger.LogInformation($"Invalid input. Please enter a valid number from the list {allowedInts}.");
        }
    }

    public int GetPlayerInputAsInt(string question, IEnumerable<int> allowedInts)
    {
        _logger.LogInformation(question);
        return GetPlayerInputAsInt(allowedInts);
    }

    public int GetPlayerInputAsIntWithMinMax(string question, int min, int max)
    {
        _logger.LogInformation(question);
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out var value) && value >= min && value <= max)
            {
                return value;
            }

            _logger.LogInformation($"Invalid input. Please enter a valid number between {min} and {max}.");
        }
    }
}
