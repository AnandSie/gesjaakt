using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public interface IGameDealerFactory
    {
        IGameDealer Create(IEnumerable<IPlayer> players);
    }
}
