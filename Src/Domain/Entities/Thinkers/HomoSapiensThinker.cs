using Domain.Entities.Players;
using Domain.Interfaces;

namespace Domain.Entities.Thinkers;

public class HomoSapiensThinker : IThinker
{
    readonly IPlayerInputProvider _playerInputProvider;
    string _name;

    public HomoSapiensThinker(IPlayerInputProvider playerInputProvider, string name)
    {
        _playerInputProvider = playerInputProvider;
        _name = name;
    }

    public TurnAction Decide(IGameStateReader gameState)
    {
        Console.Write("-----------\n");
        Console.WriteLine($"Game state is:");
        Console.WriteLine(gameState.ToString());

        Console.Write("");
        Console.WriteLine($"Hai {_name}, what do you want to do?");
        Console.WriteLine("1. Take Card 2. Play Coin");

        // TODO: create new method, instead of giving ints, give Enums
        var choice = _playerInputProvider.GetPlayerInputAsInt(new[] { 1, 2 });
        return choice switch
        {
            1 => TurnAction.TAKECARD,
            2 => TurnAction.SKIPWITHCOIN,
            _ => throw new Exception("Incorrect choice"),
        };
    }

}
