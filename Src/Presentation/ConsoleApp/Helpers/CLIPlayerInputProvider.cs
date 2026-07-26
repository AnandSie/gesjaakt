using Application.Interfaces;
using UserInterface;

namespace Presentation.ConsoleApp.Helpers;

internal class CLIPlayerInputProvider : IPlayerInputProvider
{
    public string GetPlayerInput(string question)
    {
        ConsolePrompt.Ask(question);

        while (true)
        {
            var value = Console.ReadLine();
            if (value is not null)
            {
                return value;
            }

            ConsolePrompt.Reject("Invalid input. Please enter a string.");
        }
    }

    private static int GetPlayerInputAsInt(IEnumerable<int> allowedInts)
    {
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out var value) && allowedInts.Contains(value))
            {
                return value;
            }

            ConsolePrompt.Reject($"Invalid input. Please enter a valid number from the list {string.Join(", ", allowedInts)}.");
        }
    }

    public int GetPlayerInputAsInt(string question, IEnumerable<int> allowedInts)
    {
        ConsolePrompt.Ask(question);
        return GetPlayerInputAsInt(allowedInts);
    }

    public int GetPlayerInputAsIntWithMinMax(string question, int min, int max)
    {
        ConsolePrompt.Ask(question);

        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out var value) && value >= min && value <= max)
            {
                return value;
            }

            ConsolePrompt.Reject($"Invalid input. Please enter a valid number between {min} and {max}.");
        }
    }
}
