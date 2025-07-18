using Domain.Entities.Players;
using Domain.Entities.Thinkers;
using Domain.Interfaces;

namespace Application;

public class PlayerFactory : IPlayerFactory
{
    private readonly ILogger<Player> _playerLogger;
    private readonly ILogger<HomoSapiensThinker> _thinkerLogger;

    public PlayerFactory(ILogger<Player> playerLogger, ILogger<HomoSapiensThinker> thinkerLogger)
    {
        _playerLogger = playerLogger;
        _thinkerLogger = thinkerLogger;
    }

    public IEnumerable<Func<IPlayer>> AllPlayerFactories()
    {
        return
        [
            () => new Player(new AnandThinker(), _playerLogger, "Anand"),
            () => new Player(new BartThinker(), _playerLogger, "Bart"),
            //() => new Player(new BarryThinker(), _playerLogger, "Barry"),
            //() => new Player(new BarryBeterThinker(), _playerLogger, "BarryBeter"),
            () => new Player(new BarryRealThinker(), _playerLogger, "BarryReal"),
            () => new Player(new GerardThinker(), _playerLogger, "Gerard"),
            //() => new Player(new GreedyThinker(), _playerLogger, "GreedyThinker"),
            //() => new Player(new HansThinker(), _playerLogger, "Hans"),
            () => new Player(new HansThinker_R3(), _playerLogger, "Hans_R3"),
            //() => new Player(new JensThinker(), _playerLogger, "Jens"),
            () => new Player(new JensThinker_R3(), _playerLogger, "Jens_R3"),
            //() => new Player(new JeremyThinker(), _playerLogger, "Jeremy"),
            () => new Player(new Jeremy2Thinker(), _playerLogger, "Jeremy2"),
            () => new Player(new JessieThinker_R3(), _playerLogger, "Jessie_R3"),
            //() => new Player(new JorritThinker(), _playerLogger, "Jorrit"),
            () => new Player(new JorritThinker_01(), _playerLogger, "Jorrit_01"),
            () => new Player(new JoseThinker(), _playerLogger, "Jose"),
            () => new Player(new MaartenThinker(), _playerLogger, "Maarten"),
            () => new Player(new MarijnThinker(), _playerLogger, "Marijn"),
            //() => new Player(new MatsThinker(), _playerLogger, "Mats"),
            () => new Player(new MatsThinker_R3(), _playerLogger, "Mats_R3"),
            () => new Player(new MelsThinker(), _playerLogger, "Mels"),
            () => new Player(new NilsThinker_R3(), _playerLogger, "Nils"),
            () => new Player(new OliverThinker(), _playerLogger, "Oliver"),
            () => new Player(new RubenTHinker(), _playerLogger, "Ruben"),
            () => new Player(new ScaredThinker(), _playerLogger, "ScaredThinker"),
            () => new Player(new TomasThinker(), _playerLogger, "Tomas"),
        ];
    }

    public IEnumerable<IPlayer> Create()
    {
        var players = new List<IPlayer>
        {
            //Max 7 players can be in a game simultaneously
            new Player(new AnandThinker(), _playerLogger, "Anand"),
            new Player(new BarryThinker(), _playerLogger, "Barry"),
            new Player(new BartThinker(), _playerLogger, "Bart"),
            new Player(new MarijnThinker(), _playerLogger, "Marijn"),
            new Player(new MaartenThinker(), _playerLogger, "Maarten"),
            new Player(new JeremyThinker(), _playerLogger, "Jeremy"),
            //new Player(new YourThinker(), _playerLogger, "YOURNAME") // ! Uncomment, add your thinker and name here

            //new Player(new TomasThinker(), _playerLogger, "tomas"),
            //new Player(new JensThinker(), _playerLogger, "jens") ,

            //new Player(new ScaredThinker(), _playerLogger, "ScaredThinker"), 
            //new Player(new GreedyThinker(), _playerLogger, "GreedyThinker"),
        };
        return players;
    }

    public IPlayer CreateHomoSapiens(string name, IPlayerInputProvider playerInputProvider)
    {
        var thinker = new HomoSapiensThinker(playerInputProvider, _thinkerLogger, name);
        var player = new Player(thinker, _playerLogger, name);
        return player;
    }
}