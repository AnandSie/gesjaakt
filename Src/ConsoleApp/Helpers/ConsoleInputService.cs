using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Helpers;

internal class ConsoleInputService : IPlayerInputProvider
{
    public string GetPlayerInput(string question)
    {
        Console.Write($"{question}\n");

        while (true)
        {
            var value = Console.ReadLine();
            if (value is null)
            {
                Console.WriteLine($"Invalid input. Please enter a string.");
            }
            else
            {
                return value;
            }

        }
    }

    public bool GetPlayerInputForYesNo()
    {
        Console.WriteLine("1. Yes");
        Console.WriteLine("2. No");

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

            Console.WriteLine($"Invalid input. Please enter a valid number from the list {allowedInts}.");
        }
    }

    public int GetPlayerInputAsInt(string question, IEnumerable<int> allowedInts)
    {
        Console.WriteLine(question);
        return GetPlayerInputAsInt(allowedInts);
    }
}
