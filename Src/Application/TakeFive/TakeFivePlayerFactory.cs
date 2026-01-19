using Application.Interfaces;
using Application.TakeFive.Thinkers;
using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.BaseGame;
using Domain.Interfaces.Games.TakeFive;

namespace Application.TakeFive;

public class TakeFivePlayerFactory : IPlayerFactory<ITakeFivePlayer>
{
    private readonly IPlayerInputProvider _playerInputProvider;

    public TakeFivePlayerFactory(IPlayerInputProvider playerInputProvider)
    {
        _playerInputProvider = playerInputProvider;
    }
    public IEnumerable<ITakeFivePlayer> Create()
    {
        return new List<TakeFivePlayer>
        {
            //Max 10 players can be in a game simultaneously
            //new (new YourThinker()) // ! Uncomment, add your thinker here
            new (new LisaTakeFiveThinker()),
            new (new DiverTakeFiveThinker()),
            new (new BlindTakeFiveThinker()),
        };
    }

    public IEnumerable<Func<ITakeFivePlayer>> AllPlayerFactories()
    {
        // REFACTOR - use DI/REFLECTION to auto create this
        return new List<Func<TakeFivePlayer>>
        {
            () => new (new LisaTakeFiveThinker()),
            () => new (new DiverTakeFiveThinker()),
            () => new (new BlindTakeFiveThinker()),
        };
    }

    public ITakeFivePlayer CreateManualPlayer()
    {
        var name = _playerInputProvider.GetPlayerInput($"Next player, what is your name?");
        var thinker = new ManualTakeFiveThinker(_playerInputProvider, name);
        var player = new TakeFivePlayer(thinker);
        return player;
    }

    public IEnumerable<ITakeFivePlayer> CreateManualPlayers(int playersToAdd)
    {
        var players = new List<ITakeFivePlayer>();
        foreach (var i in Enumerable.Range(TakeFiveRules.MinNumberOfPlayers, playersToAdd))
        {
            players.Add(CreateManualPlayer());
        }
        return players;
    }
}
