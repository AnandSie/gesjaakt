using Domain.Entities.Players;
using Domain.Entities.Thinkiers;
using Domain.Interfaces;

namespace Application
{
    public class PlayerFactory : IPlayerFactory
    {
        public IEnumerable<IPlayer> Create()
        {
            var players = new List<IPlayer>
            {
                new Player(new BartThinker(), "Bart"),
                new Player(new AnandThinker(), "Anand"),
                new Player(new MaartenThinker(), "Maarten"),
                new Player(new TomasThinker(), "Tomas")
            };
            return players;
        }
    }
}