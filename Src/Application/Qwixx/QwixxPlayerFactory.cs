using Application.Interfaces;
using Application.Qwixx.Thinkers;
using Domain.Entities.Game.Qwixx;
using Domain.Interfaces.Games.BaseGame;
using Domain.Interfaces.Games.Qwixx;

namespace Application.Qwixx;

public class QwixxPlayerFactory : IPlayerFactory<IQwixxPlayer>
{
    private readonly IPlayerInputProvider _playerInputProvider;

    public QwixxPlayerFactory(IPlayerInputProvider playerInputProvider)
    {
        _playerInputProvider = playerInputProvider;
    }

    public IEnumerable<IQwixxPlayer> Create()
    {
        return new List<QwixxPlayer>
        {
            // Max 5 players can be in a game simultaneously
            new(new GreedyQwixxThinker("Greedy1")),
            new(new GreedyQwixxThinker("Greedy2")),
            new(new GreedyQwixxThinker("Greedy3")),
            //new(new YourThinker()) // ! Uncomment, add your thinker here
        };
    }

    public IEnumerable<Func<IQwixxPlayer>> AllPlayerFactories()
    {
        // REFACTOR - use DI/REFLECTION to auto create this
        return new List<Func<QwixxPlayer>>
        {
            () => new(new GreedyQwixxThinker("Greedy1")),
            () => new(new GreedyQwixxThinker("Greedy2")),
            () => new(new GreedyQwixxThinker("Greedy3")),
        };
    }

    public IQwixxPlayer CreateManualPlayer()
    {
        var name = _playerInputProvider.GetPlayerInput("Next player, what is your name?");
        var thinker = new ManualQwixxThinker(_playerInputProvider, name);
        return new QwixxPlayer(thinker);
    }

    public IEnumerable<IQwixxPlayer> CreateManualPlayers(int playersToAdd)
    {
        var players = new List<IQwixxPlayer>();
        foreach (var i in Enumerable.Range(QwixxRules.MinNumberOfPlayers, playersToAdd))
        {
            players.Add(CreateManualPlayer());
        }
        return players;
    }
}
