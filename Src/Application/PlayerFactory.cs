using Domain.Entities.Game;
using Domain.Entities.Players;
using Domain.Entities.Thinkiers;
using Domain.Interfaces;

namespace Application
{
    public class PlayerFactory : IPlayerFactory
    {
        private readonly ILogger<Player> _logger;

        public PlayerFactory(ILogger<Player> logger)
        {
            _logger = logger;
        }

        public IEnumerable<IPlayer> Create()
        {
            var players = new List<IPlayer>
            {
                new Player(new BartThinker(), _logger, "Bart"),
                new Player(new AnandThinker(), _logger, "Anand"),
                new Player(new MaartenThinker(), _logger, "Maarten"),
                new Player(new TomasThinker(), _logger, "Tomas")
            };
            return players;
        }

        public IPlayer CreateHomoSapiens(string name, IPlayerInputProvider playerInputProvider)
        {
            var player = new Player(new HomoSapiensThinker(playerInputProvider, name), _logger, name); // FIXME Both classes uses name

            return player;
        }
    }
}