using System.Reflection;
using Domain.Entities.Players;
using Domain.Entities.Thinkiers;
using Domain.Interfaces;

namespace Application
{
    public class PlayerFactory : IPlayerFactory
    {
        string dynamicPath = "../thinkers";
        IEnumerable<IPlayer> _dynamicPlayers;

        public IEnumerable<IPlayer> Create()
        {
            var players = _dynamicPlayers.ToList();
            players.Add(new Player(new TemplateThinker(), "Template"));

            return players;
        }


        public bool LoadDynamicThinkers()
        {
            List<IPlayer> players = new List<IPlayer>();
            bool success = true;

            List<string> files = new List<string>();
            files = Directory.GetFiles(dynamicPath, "*.cs").ToList<string>();
            var compiler = new DynamicThinkerCompiler();

            foreach (string file in files)
            {
                string n = Path.GetFileNameWithoutExtension(file); //new FileInfo(file).Name;

                try
                {
                    IThinker t = compiler.LoadThinkerFromFile(file);
                    players.Add(new Player(t, n));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to compile file '{n}.cs': {e.GetType().ToString()}: {e.Message}.");
                    success = false;
                }
            }

            _dynamicPlayers = players.ToArray();
            return success;
        }
    }
}