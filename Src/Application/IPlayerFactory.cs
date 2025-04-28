using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application;

public interface IPlayerFactory
{
    public IEnumerable<IPlayer> Create();
    public IPlayer CreateHomoSapiens(string name, IPlayerInputProvider playerInputProvider);
}
