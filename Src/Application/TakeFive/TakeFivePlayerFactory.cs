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
            // TODO - Consider giving the TakeFiveThinker a "Name" and use this Name for the player as well (then you only have to define everything in one place)
            //Max 10 players can be in a game simultaneously
            //new (new YourThinker(),  "YOURNAME") // ! Uncomment, add your thinker and name here
            new (new LisaTakeFiveThinker(), name: "Lisa"),
            new (new DiverTakeFiveThinker(), name: "Diver"),
            new (new BlindTakeFiveThinker(), name: "Blind"),
        };
    }

    public IEnumerable<Func<ITakeFivePlayer>> AllPlayerFactories()
    {
        // REFACTOR - use DI/REFLECTION to auto create this
        return new List<Func<TakeFivePlayer>>
        {
            () => new (new LisaTakeFiveThinker(), name: "Lisa"),
            () => new (new DiverTakeFiveThinker(), name: "Diver"),
            () => new (new BlindTakeFiveThinker(), name: "Blind"),
        };
    }

    public ITakeFivePlayer CreateManualPlayer()
    {
        var name = _playerInputProvider.GetPlayerInput($"Next player, what is your name?");
        var thinker = new ManualTakeFiveThinker(_playerInputProvider, name);
        var player = new TakeFivePlayer(thinker, name);
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
