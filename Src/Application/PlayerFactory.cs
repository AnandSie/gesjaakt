using Domain.Entities.Players;
using Domain.Entities.Thinkers;
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
                new Player(new BartThinker(), "Bart"),
                new Player(new AnandThinker(), "Anand"),
                new Player(new MarijnThinker(), "Marijn"),
                new Player(new TomasThinker(), "Tomas"),
                new Player(new MaartenThinker(), "Maarten"),
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