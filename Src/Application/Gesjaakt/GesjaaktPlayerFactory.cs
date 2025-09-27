using Application.Interfaces;
using Domain.Entities.Game.Gesjaakt;
using Domain.Entities.Game.Gesjaakt.Thinkers;
using Domain.Entities.Thinkers;
using Domain.Interfaces;
using Domain.Interfaces.Games.BaseGame;
using Domain.Interfaces.Games.Gesjaakt;

namespace Application.Gesjaakt;

public class GesjaaktPlayerFactory : IPlayerFactory<IGesjaaktPlayer>
{
    private readonly IPlayerInputProvider _playerInputProvider;

    public GesjaaktPlayerFactory(ILogger<GesjaaktPlayer> playerLogger, IPlayerInputProvider playerInputProvider)
    {
        _playerInputProvider = playerInputProvider;
    }

    public IEnumerable<Func<IGesjaaktPlayer>> AllPlayerFactories()
    {
        return
        [
            () => new GesjaaktPlayer(new AnandThinker(),  "Anand"),
            () => new GesjaaktPlayer(new BartThinker(),  "Bart"),
            //() => new Player(new BarryThinker(),  "Barry"),
            //() => new Player(new BarryBeterThinker(),  "BarryBeter"),
            () => new GesjaaktPlayer(new BarryRealThinker(),  "BarryReal"),
            () => new GesjaaktPlayer(new GerardThinker(),  "Gerard"),
            //() => new Player(new GreedyThinker(),  "GreedyThinker"),
            //() => new Player(new HansThinker(),  "Hans"),
            () => new GesjaaktPlayer(new HansThinker_R3(),  "Hans_R3"),
            //() => new Player(new JensThinker(),  "Jens"),
            () => new GesjaaktPlayer(new JensThinker_R3(),  "Jens_R3"),
            //() => new Player(new JeremyThinker(),  "Jeremy"),
            () => new GesjaaktPlayer(new Jeremy2Thinker(),  "Jeremy2"),
            () => new GesjaaktPlayer(new JessieThinker_R3(),  "Jessie_R3"),
            //() => new Player(new JorritThinker(),  "Jorrit"),
            () => new GesjaaktPlayer(new JorritThinker_01(),  "Jorrit_01"),
            () => new GesjaaktPlayer(new JoseThinker(),  "Jose"),
            () => new GesjaaktPlayer(new MaartenThinker(),  "Maarten"),
            () => new GesjaaktPlayer(new MarijnThinker(),  "Marijn"),
            //() => new Player(new MatsThinker(),  "Mats"),


            //() => new Player(new MatsThinker_R3(),  "Mats_R3"),
            //() => new Player(new MelsThinker(),  "Mels"),
            //() => new Player(new NilsThinker_R3(),  "Nils"),
            //() => new Player(new OliverThinker(),  "Oliver"),
            //() => new Player(new RubenTHinker(),  "Ruben"),
            //() => new Player(new ScaredThinker(),  "ScaredThinker"),
            //() => new Player(new TomasThinker(),  "Tomas"),
        ];
    }

    public IEnumerable<IGesjaaktPlayer> Create()
    {
        var players = new List<IGesjaaktPlayer>
        {
            //Max 7 players can be in a game simultaneously
            new GesjaaktPlayer(new AnandThinker(),  "Anand"),
            new GesjaaktPlayer(new BarryThinker(),  "Barry"),
            new GesjaaktPlayer(new BartThinker(),  "Bart"),
            new GesjaaktPlayer(new MarijnThinker(),  "Marijn"),
            new GesjaaktPlayer(new MaartenThinker(),  "Maarten"),
            new GesjaaktPlayer(new JeremyThinker(),  "Jeremy"),
            //new Player(new YourThinker(),  "YOURNAME") // ! Uncomment, add your thinker and name here

            //new Player(new TomasThinker(),  "tomas"),
            //new Player(new JensThinker(),  "jens") ,

            //new Player(new ScaredThinker(),  "ScaredThinker"), 
            //new Player(new GreedyThinker(),  "GreedyThinker"),
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
        foreach (var i in Enumerable.Range(3, playersToAdd))
        {
            players.Add(CreateManualPlayer());
        }
        return players;

    }
}