using Application.Gesjaakt.Thinkers;
using Application.Interfaces;
using Application.TakeFive.Thinkers;
using Domain.Entities.Game.Gesjaakt;
using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.BaseGame;
using Domain.Interfaces.Games.Gesjaakt;
using Domain.Interfaces.Games.TakeFive;

namespace Application.TakeFive;

public class TakeFivePlayerFactory : IPlayerFactory<ITakeFivePlayer>
{
    private readonly IPlayerInputProvider _playerInputProvider;

    public TakeFivePlayerFactory(IPlayerInputProvider playerInputProvider)
    {
        _playerInputProvider = playerInputProvider;
    }

    public IEnumerable<Func<ITakeFivePlayer>> AllPlayerFactories()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ITakeFivePlayer> Create()
    {
        throw new NotImplementedException();
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
