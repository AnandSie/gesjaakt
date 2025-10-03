
using Application.Gesjaakt.Thinkers;
using Application.Interfaces;
using Domain.Entities.Game.Gesjaakt;
using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.BaseGame;
using Domain.Interfaces.Games.Gesjaakt;

namespace Application.Gesjaakt;

public class GesjaaktPlayerFactory : IPlayerFactory<IGesjaaktPlayer>
{
    private readonly IPlayerInputProvider _playerInputProvider;

    public GesjaaktPlayerFactory(IPlayerInputProvider playerInputProvider)
    {
        _playerInputProvider = playerInputProvider;
    }

    public IEnumerable<Func<IGesjaaktPlayer>> AllPlayerFactories()
    {
        // REFACTOR - use DI/REFLECTION to auto create this

        return new List<Func<GesjaaktPlayer>>
        {
            () => new (new AnandThinker(),  "Anand"),
            () => new (new BartThinker(),  "Bart"),
            //() => new (new BarryThinker(),  "Barry"),
            //() => new (new BarryBeterThinker(),  "BarryBeter"),
            () => new (new BarryRealThinker(),  "BarryReal"),
            () => new (new GerardThinker(),  "Gerard"),
            //() => new (new GreedyThinker(),  "GreedyThinker"),
            //() => new (new HansThinker(),  "Hans"),
            () => new (new HansThinker_R3(),  "Hans_R3"),
            //() => new (new JensThinker(),  "Jens"),
            () => new (new JensThinker_R3(),  "Jens_R3"),
            //() => new (new JeremyThinker(),  "Jeremy"),
            () => new (new Jeremy2Thinker(),  "Jeremy2"),
            () => new (new JessieThinker_R3(),  "Jessie_R3"),
            //() => new (new JorritThinker(),  "Jorrit"),
            () => new (new JorritThinker_01(),  "Jorrit_01"),
            () => new (new JoseThinker(),  "Jose"),
            () => new (new MaartenThinker(),  "Maarten"),
            () => new (new MarijnThinker(),  "Marijn"),
            //() => new (new MatsThinker(),  "Mats"),

            //() => new (new MatsThinker_R3(),  "Mats_R3"),
            //() => new (new MelsThinker(),  "Mels"),
            //() => new (new NilsThinker_R3(),  "Nils"),
            //() => new (new OliverThinker(),  "Oliver"),
            //() => new (new RubenTHinker(),  "Ruben"),
            //() => new (new ScaredThinker(),  "ScaredThinker"),
            //() => new (new TomasThinker(),  "Tomas"),
        };
    }

    public IEnumerable<IGesjaaktPlayer> Create()
    {
        var players = new List<GesjaaktPlayer>
        {
            //Max 7 players can be in a game simultaneously
            new (new AnandThinker(),  "Anand"),
            new (new BarryThinker(),  "Barry"),
            new (new BartThinker(),  "Bart"),
            new (new MarijnThinker(),  "Marijn"),
            new (new MaartenThinker(),  "Maarten"),
            new (new JeremyThinker(),  "Jeremy"),
            //new (new YourThinker(),  "YOURNAME") // ! Uncomment, add your thinker and name here

            //new (new TomasThinker(),  "tomas"),
            //new (new JensThinker(),  "jens") ,
            //new (new ScaredThinker(),  "ScaredThinker"), 
            //new (new GreedyThinker(),  "GreedyThinker"),
        };
        return players;
    }

    public IGesjaaktPlayer CreateManualPlayer()
    {
        var name = _playerInputProvider.GetPlayerInput($"Next player, what is your name?");
        var thinker = new ManualGesjaaktThinker(_playerInputProvider, name);
        var player = new GesjaaktPlayer(thinker, name);
        return player;
    }

    public IEnumerable<IGesjaaktPlayer> CreateManualPlayers(int playersToAdd)
    {
        var players = new List<IGesjaaktPlayer>();
        foreach (var i in Enumerable.Range(3, playersToAdd)) // REFACTOR - GESJAAKTRULES
        {
            players.Add(CreateManualPlayer());
        }
        return players;
    }
}