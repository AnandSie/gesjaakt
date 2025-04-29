using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public interface IGameStateFactory
    {
        IGameState Create(IEnumerable<IPlayer> players);
    }
}
