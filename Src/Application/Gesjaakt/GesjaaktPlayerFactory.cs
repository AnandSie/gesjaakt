using Domain.Entities.Game.Gesjaakt;
using Domain.Entities.Game.Gesjaakt.Thinkers;
using Domain.Entities.Thinkers;
using Domain.Interfaces;
using Domain.Interfaces.Games.BaseGame;
using Domain.Interfaces.Games.Gesjaakt;

namespace Application.Gesjaakt;

public class GesjaaktPlayerFactory : IPlayerFactory<IGesjaaktPlayer>
{
    private readonly ILogger<GesjaaktPlayer> _playerLogger;
    private readonly ILogger<ManualGesjaaktThinker> _thinkerLogger;
    private readonly IPlayerInputProvider _playerInputProvider;

    public GesjaaktPlayerFactory(ILogger<GesjaaktPlayer> playerLogger, ILogger<ManualGesjaaktThinker> thinkerLogger, IPlayerInputProvider playerInputProvider)
    {
        _playerLogger = playerLogger;
        _thinkerLogger = thinkerLogger;
        _playerInputProvider = playerInputProvider;
    }

    public IEnumerable<Func<IGesjaaktPlayer>> AllPlayerFactories()
    {
        return
        [
            () => new GesjaaktPlayer(new AnandThinker(), _playerLogger, "Anand"),
            () => new GesjaaktPlayer(new BartThinker(), _playerLogger, "Bart"),
            //() => new Player(new BarryThinker(), _playerLogger, "Barry"),
            //() => new Player(new BarryBeterThinker(), _playerLogger, "BarryBeter"),
            () => new GesjaaktPlayer(new BarryRealThinker(), _playerLogger, "BarryReal"),
            () => new GesjaaktPlayer(new GerardThinker(), _playerLogger, "Gerard"),
            //() => new Player(new GreedyThinker(), _playerLogger, "GreedyThinker"),
            //() => new Player(new HansThinker(), _playerLogger, "Hans"),
            () => new GesjaaktPlayer(new HansThinker_R3(), _playerLogger, "Hans_R3"),
            //() => new Player(new JensThinker(), _playerLogger, "Jens"),
            () => new GesjaaktPlayer(new JensThinker_R3(), _playerLogger, "Jens_R3"),
            //() => new Player(new JeremyThinker(), _playerLogger, "Jeremy"),
            () => new GesjaaktPlayer(new Jeremy2Thinker(), _playerLogger, "Jeremy2"),
            () => new GesjaaktPlayer(new JessieThinker_R3(), _playerLogger, "Jessie_R3"),
            //() => new Player(new JorritThinker(), _playerLogger, "Jorrit"),
            () => new GesjaaktPlayer(new JorritThinker_01(), _playerLogger, "Jorrit_01"),
            () => new GesjaaktPlayer(new JoseThinker(), _playerLogger, "Jose"),
            () => new GesjaaktPlayer(new MaartenThinker(), _playerLogger, "Maarten"),
            () => new GesjaaktPlayer(new MarijnThinker(), _playerLogger, "Marijn"),
            //() => new Player(new MatsThinker(), _playerLogger, "Mats"),


            //() => new Player(new MatsThinker_R3(), _playerLogger, "Mats_R3"),
            //() => new Player(new MelsThinker(), _playerLogger, "Mels"),
            //() => new Player(new NilsThinker_R3(), _playerLogger, "Nils"),
            //() => new Player(new OliverThinker(), _playerLogger, "Oliver"),
            //() => new Player(new RubenTHinker(), _playerLogger, "Ruben"),
            //() => new Player(new ScaredThinker(), _playerLogger, "ScaredThinker"),
            //() => new Player(new TomasThinker(), _playerLogger, "Tomas"),
        ];
    }

    public IEnumerable<IGesjaaktPlayer> Create()
    {
        var players = new List<IGesjaaktPlayer>
        {
            //Max 7 players can be in a game simultaneously
            new GesjaaktPlayer(new AnandThinker(), _playerLogger, "Anand"),
            new GesjaaktPlayer(new BarryThinker(), _playerLogger, "Barry"),
            new GesjaaktPlayer(new BartThinker(), _playerLogger, "Bart"),
            new GesjaaktPlayer(new MarijnThinker(), _playerLogger, "Marijn"),
            new GesjaaktPlayer(new MaartenThinker(), _playerLogger, "Maarten"),
            new GesjaaktPlayer(new JeremyThinker(), _playerLogger, "Jeremy"),
            //new Player(new YourThinker(), _playerLogger, "YOURNAME") // ! Uncomment, add your thinker and name here

            //new Player(new TomasThinker(), _playerLogger, "tomas"),
            //new Player(new JensThinker(), _playerLogger, "jens") ,

            //new Player(new ScaredThinker(), _playerLogger, "ScaredThinker"), 
            //new Player(new GreedyThinker(), _playerLogger, "GreedyThinker"),
        };
        return players;
    }

    public IGesjaaktPlayer CreateHomoSapiens()
    {
        var name = _playerInputProvider.GetPlayerInput($"Next player, what is your name?");
        var thinker = new ManualGesjaaktThinker(_playerInputProvider, _thinkerLogger, name);
        var player = new GesjaaktPlayer(thinker, _playerLogger, name);
        return player;
    }

    public IEnumerable<IGesjaaktPlayer> CreateManualPlayers(int playersToAdd)
    {
        var players = new List<IGesjaaktPlayer>();
        foreach (var i in Enumerable.Range(3, playersToAdd))
        {
            players.Add(CreateHomoSapiens());
        }
        return players;

    }
}