using Domain.Entities.Game.Gesjaakt;
using Domain.Interfaces;
using System.Text;

namespace Domain.Entities.Thinkers;

public class HomoSapiensThinker : IThinker
{
    readonly IPlayerInputProvider _playerInputProvider;
    readonly ILogger<HomoSapiensThinker> _logger;
    string _name;

    public HomoSapiensThinker(IPlayerInputProvider playerInputProvider, ILogger<HomoSapiensThinker> logger, string name)
    {
        _playerInputProvider = playerInputProvider;
        _name = name;
        _logger = logger;
    }

    public GesjaaktTurnOption Decide(IGesjaaktReadOnlyGameState gameState)
    {
        var logMessage = new StringBuilder();

        logMessage.AppendLine("---GAME STATE---");
        logMessage.AppendLine(gameState.ToString());
        logMessage.AppendLine();
        logMessage.AppendLine($"Hi {_name}, what do you want to do?");
        logMessage.AppendLine("1. Take Card  2. Play Coin");

        _logger.LogCritical(logMessage.ToString());

        // TODO: create new method, instead of giving ints, give Enums
        var choice = _playerInputProvider.GetPlayerInputAsInt(new[] { 1, 2 });
        return choice switch
        {
            1 => GesjaaktTurnOption.TAKECARD,
            2 => GesjaaktTurnOption.SKIPWITHCOIN,
            _ => throw new Exception("Incorrect choice"),
        };
    }

}
