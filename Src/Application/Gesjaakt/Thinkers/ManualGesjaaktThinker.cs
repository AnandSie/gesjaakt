using Application.Interfaces;
using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces.Games.Gesjaakt;
using System.Text;

namespace Application.Gesjaakt.Thinkers;

public class ManualGesjaaktThinker(IPlayerInputProvider playerInputProvider, string name) : IGesjaaktThinker
{
    public GesjaaktTurnOption Decide(IGesjaaktReadOnlyGameState gameState)
    {
        var question = new StringBuilder();

        question.AppendLine("---GAME STATE---");
        question.AppendLine(gameState.ToString());
        question.AppendLine();
        question.AppendLine($"Hi {name}, what do you want to do?");
        question.AppendLine("1. Take Card  2. Play Coin");

        // REFACTOR: create new method, instead of giving ints, give Enums
        var choice = playerInputProvider.GetPlayerInputAsInt(question.ToString(),[1, 2]);
        return choice switch
        {
            1 => GesjaaktTurnOption.TAKECARD,
            2 => GesjaaktTurnOption.SKIPWITHCOIN,
            _ => throw new Exception("Incorrect choice"),
        };
    }
}
