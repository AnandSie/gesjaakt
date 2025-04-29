using Domain.Entities.Players;
using Domain.Entities.Thinkers;
using Domain.Interfaces;

namespace Application
{
    public class PlayerFactory : IPlayerFactory
    {
        public IEnumerable<IPlayer> Create()
        {
            var players = new List<IPlayer>
            {
                new Player(new AnandThinker(), "Anand"),
                new Player(new BarryThinker(), "Barry"),
                new Player(new BartThinker(), "Bart"),
                new Player(new MarijnThinker(), "Marijn"),
                new Player(new TomasThinker(), "Tomas"),
                new Player(new MaartenThinker(), "Maarten"),
                new Player(new JeremyThinker(), "Jeremy"),
                //new Player(new ScaredThinker(), "ScaredThinker"), //Max 7 players can be in a game simultaneously
                //new Player(new GreedyThinker(), "GreedyThinker"),
                //new Player(new JensThinker(), "Jens") ,
            };
            return players;
        }
    }
}