using Domain.Entities.Players;
using Domain.Entities.Thinkers;
using Domain.Entities.Thinkiers;
using Domain.Interfaces;

namespace Application
{
    public class PlayerFactory : IPlayerFactory
    {
        private readonly ILogger<Player> _playerLogger;
        private readonly ILogger<HomoSapiensThinker> _thinkerLogger;

        public PlayerFactory(ILogger<Player> playerLogger, ILogger<HomoSapiensThinker> thinkerLogger)
        {
            _playerLogger = playerLogger;
            _thinkerLogger = thinkerLogger;
        }

        public IEnumerable<IPlayer> Create()
        {
            var players = new List<IPlayer>
        {
            new Player(new BartThinker(), _playerLogger, "Bart"),
            new Player(new AnandThinker(), _playerLogger, "Anand"),
            new Player(new MarijnThinker(), _playerLogger, "Marijn"),
            new Player(new TomasThinker(), _playerLogger, "Tomas"),
            new Player(new MaartenThinker(), _playerLogger, "Maarten"),
            new Player(new BarryThinker(), _playerLogger, "Barry"),
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
}